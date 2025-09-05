using System.Text.Json;
using System.Text.RegularExpressions;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IchigoHoshimiya.BackgroundServices;

public partial class RssSearcherAndPosterService(
    IServiceScopeFactory scopeFactory,
    HttpClient httpClient,
    IConfiguration configuration,
    IClient client)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessRssResults(stoppingToken);

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async Task ProcessRssResults(CancellationToken stoppingToken)
    {
        var jackettUrl = configuration["JackettUrl"];

        if (string.IsNullOrWhiteSpace(jackettUrl))
        {
            Console.WriteLine("JackettUrl is not configured.");

            return;
        }

        using var scope = scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IchigoContext>();

        var reminders = await dbContext.RssReminder.Where(reminder => reminder.Enabled).ToListAsync(stoppingToken);

        foreach (var reminder in reminders)
        {
            var encodedQuery = Uri.EscapeDataString(reminder.SearchString);
            var fullUrl = $"{jackettUrl}&Query={encodedQuery}&_={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

            try
            {
                var response = await httpClient.GetAsync(fullUrl, stoppingToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(stoppingToken);

                JackettResponse? result;

                try
                {
                    result = JsonSerializer.Deserialize<JackettResponse>(content);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Failed to deserialize Jackett response: {ex.Message}");

                    continue;
                }

                if (result?.Results is not null)
                {
                    var regexPattern = BuildSearchPattern(reminder.SearchString);

                    var matches = result.Results
                                        .Where(j => regexPattern.IsMatch(j.Title))
                                        .ToList();

                    if (matches.Count > 0)
                    {
                        var matchedRecord = matches.First();

                        reminder.Enabled = false;

                        await client.SendMessageAsync(
                            (ulong)reminder.ChannelId,
                            $"Hey <@{reminder.CreatedById}> {reminder.Mentions}, **{reminder.SearchString}** just dropped:\n{matchedRecord.Details}\n{matchedRecord.InfoHash}");

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error calling jackett for '{reminder.SearchString}': {ex.Message}");
            }
        }
    }

    private static Regex BuildSearchPattern(string searchString)
    {
        var normalizedSearchString = NormalizeSearchString(searchString);

        var searchTerms = normalizedSearchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var pattern = string.Join(".*", searchTerms.Select(term => @$"\b{Regex.Escape(term)}\b"));

        return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static string NormalizeSearchString(string input)
    {
        var normalized = input.ToLowerInvariant();

        normalized = NonAlphanumericRegex().Replace(normalized, " ");

        return SpaceReducerRegex().Replace(normalized, " ").Trim();
    }

    [GeneratedRegex("[^a-zA-Z0-9 ]+")]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex SpaceReducerRegex();

    private class JackettResponse
    {
        public List<JackettResult> Results { get; init; } = [];
    }

    private class JackettResult
    {
        [UsedImplicitly] public string Title { get; set; } = string.Empty;

        [UsedImplicitly] public string Details { get; set; } = string.Empty;

        [UsedImplicitly] public string InfoHash { get; set; } = string.Empty;
    }
}