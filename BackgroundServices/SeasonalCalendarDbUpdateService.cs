using System.Text;
using System.Text.Json;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.Entities.AniList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IchigoHoshimiya.BackgroundServices;

public record AnilistResponse(AnilistData Data);

public record AnilistData(Page Page);

public record Page(PageInfo PageInfo, List<Media> Media);

public record PageInfo(bool HasNextPage);

public record Media(
    long Id,
    Title Title,
    AiringSchedule AiringSchedule);

public record Title(string Romaji);

public record AiringSchedule(List<AiringNode> Nodes);

public record AiringNode(long AiringAt, int Episode);

public class SeasonalCalendarDbUpdateService(
    ILogger<AnimeThemesDbUpdateService> logger,
    HttpClient httpClient,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("AnimeThemes Update Service is starting.");

        // Run immediately at startup
        await GetSeasonalCalendarAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeSpan.FromHours(2);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break; // Service is stopping
            }

            await GetSeasonalCalendarAsync(stoppingToken);
        }
    }

    private async Task GetSeasonalCalendarAsync(CancellationToken stoppingToken)
    {
        var graphQlEndpoint = "https://graphql.anilist.co";

        var query = await File.ReadAllTextAsync("Queries/Anilist/SeasonalsQuery.graphql", stoppingToken);

        var (startDate, endDate) = GetCurrentYearFuzzyDates();

        // 3. Prepare for pagination
        var allMedia = new List<Media>();
        var page = 1;
        var hasNextPage = true;

        Console.WriteLine($"Fetching seasonal anime between {startDate} and {endDate}...");

        while (hasNextPage && !stoppingToken.IsCancellationRequested)
        {
            var requestBody = new
            {
                query,
                variables = new
                {
                    startdateGreater = startDate,
                    startdateLesser = endDate,
                    page
                }
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(graphQlEndpoint, content, stoppingToken);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync(stoppingToken);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var anilistResponse =
                await JsonSerializer.DeserializeAsync<AnilistResponse>(responseStream, options, stoppingToken);

            if (anilistResponse?.Data.Page.Media is not null)
            {
                allMedia.AddRange(anilistResponse.Data.Page.Media);
                Console.WriteLine($"Page {page}: Fetched {anilistResponse.Data.Page.Media.Count} entries.");
            }

            hasNextPage = anilistResponse?.Data.Page.PageInfo.HasNextPage ?? false;
            page++;

            await Task.Delay(1000, stoppingToken);
        }

        Console.WriteLine($"Finished fetching. Total entries: {allMedia.Count}");

        var anilistIds = allMedia.Select(m => m.Id).ToHashSet();
        
        using var scope = scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IchigoContext>();

        var existingAnime = await dbContext.AiringAnime
                                           .Include(a => a.AiringEpisodes)
                                           .Where(a => anilistIds.Contains(a.AnilistId))
                                           .ToDictionaryAsync(a => a.AnilistId, stoppingToken);

        var animeToAdd = new List<AiringAnime>();

        foreach (var media in allMedia)
        {
            if (existingAnime.TryGetValue(media.Id, out var ea))
            {
                // In case the db changes the title, let's update it in any case
                ea.Title = media.Title.Romaji;
                
                var existingEpisodes = ea.AiringEpisodes.ToDictionary(e => e.EpisodeNumber);

                foreach (var scheduleNode in media.AiringSchedule.Nodes)
                {
                    if (existingEpisodes.TryGetValue(scheduleNode.Episode, out var existingEpisode))
                    {
                        // Update episode if airing time changed
                        existingEpisode.AiringAtUtc =
                            DateTimeOffset.FromUnixTimeSeconds(scheduleNode.AiringAt).UtcDateTime;
                    }
                    else
                    {
                        // Add new episodes that popped up
                        ea.AiringEpisodes.Add(
                            new AiringEpisode
                            {
                                EpisodeNumber = scheduleNode.Episode,
                                AiringAtUtc = DateTimeOffset.FromUnixTimeSeconds(scheduleNode.AiringAt).UtcDateTime
                            });
                    }
                }
            }
            else
            {
                var newAnime = new AiringAnime
                {
                    AnilistId = media.Id,
                    Title = media.Title.Romaji,
                    AiringEpisodes = media.AiringSchedule.Nodes.Select(e => new AiringEpisode
                                           {
                                               EpisodeNumber = e.Episode,
                                               AiringAtUtc = DateTimeOffset.FromUnixTimeSeconds(e.AiringAt).UtcDateTime
                                           })
                                          .ToList()
                };

                animeToAdd.Add(newAnime);
            }
        }
        
        if (animeToAdd.Count != 0)
        {
            await dbContext.AiringAnime.AddRangeAsync(animeToAdd, stoppingToken);
        }
        
        var changes = await dbContext.SaveChangesAsync(stoppingToken);
        Console.WriteLine($"Database synchronization complete. {changes} changes saved.");
    }

    // Look into this, is this really the best way to get what needs to be stored
    private static (int startDate, int endDate) GetCurrentYearFuzzyDates()
    {
        var (year, _, _) = DateTime.UtcNow;

        return (int.Parse($"{year}0101"), int.Parse($"{year + 1}1231"));
    }
}