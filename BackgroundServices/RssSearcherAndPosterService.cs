using System.Text.Json;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IchigoHoshimiya.BackgroundServices;

public class RssSearcherAndPosterService(
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
                    var matches = result.Results
                                        .Where(j => j.Title.Contains(
                                             reminder.SearchString,
                                             StringComparison.OrdinalIgnoreCase))
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

    private class JackettResponse
    {
        public List<JackettResult> Results { get; init; } = [];
    }

    private class JackettResult
    {
        public string Title { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string InfoHash { get; set; } = string.Empty;
    }
}