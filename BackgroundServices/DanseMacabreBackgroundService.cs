using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Rest;

namespace IchigoHoshimiya.BackgroundServices;

public class DanseMacabreBackgroundService(RestClient restClient) : BackgroundService
{
    private readonly DateTimeOffset _cutoffDate = new(2025, 4, 1, 0, 0, 0, TimeSpan.Zero);
    private readonly ulong _guildId = 514203145333899276;

    private readonly HashSet<ulong> _ignoredChannelIds =
    [
        514215944319401996,
        1083446590833963018,
        718258937228558389,
        1009979267632865310,
        1332544712657076264
    ];

    private readonly DateTimeOffset _lowerBoundDate = new(2022, 2, 19, 0, 0, 0, TimeSpan.Zero);
    private readonly ulong _userId = 291678129586438144;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IEnumerable<TextGuildChannel> filtered;

        try
        {
            var channels = await restClient.GetGuildChannelsAsync(_guildId, cancellationToken: stoppingToken);

            filtered = channels
                      .OfType<TextGuildChannel>()
                      .Where(c => _ignoredChannelIds.Contains(c.Id));

            foreach (var channel in filtered)
            {
                Console.WriteLine($"{channel.Id} {channel.Name}");
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to fetch guild channels: {ex.Message}. Service will terminate.");

            return;
        }

        foreach (var channel in filtered)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            await ProcessChannelAsync(channel, stoppingToken);
        }

        Console.WriteLine("Danse Macabre Background Service finished execution.");
    }

    private async Task ProcessChannelAsync(TextGuildChannel channel, CancellationToken stoppingToken)
    {
        Console.WriteLine($"Starting processing channel: {channel.Name} ({channel.Id})");

        DateTimeOffset? beforeDateTime = _cutoffDate;
        var reachedLowerBound = false;

        while (!stoppingToken.IsCancellationRequested && !reachedLowerBound)
        {
            if (beforeDateTime <= _lowerBoundDate)
            {
                Console.WriteLine($"Reached lower bound in {channel.Id}");

                break;
            }

            var pagination = new PaginationProperties<ulong>
            {
                From = DateTimeToSnowflake(beforeDateTime.Value),
                Direction = PaginationDirection.Before,
                BatchSize = 100
            };

            var foundAny = false;

            try
            {
                await foreach (var message in channel.GetMessagesAsync(pagination).WithCancellation(stoppingToken))
                {
                    foundAny = true;

                    if (message.CreatedAt <= _lowerBoundDate)
                    {
                        reachedLowerBound = true;

                        break;
                    }

                    if (message.Author.Id == _userId && message.CreatedAt < _cutoffDate)
                    {
                        try
                        {
                            await message.DeleteAsync(cancellationToken: stoppingToken);

                            // just don't go crazy with it ig
                            await Task.Delay(100, stoppingToken);

                            Console.WriteLine(
                                $"A message at {message.CreatedAt} in channel {message.ChannelId} was deleted");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to delete {message.Id} in {channel.Id}: {ex.Message}");
                        }
                    }

                    beforeDateTime = message.CreatedAt;

                    Console.WriteLine(
                        $"Currently reading a message at {message.CreatedAt} in channel {message.ChannelId}");

                    // just don't go crazy with it ig
                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"Processing for channel {channel.Id} cancelled by token.");

                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"An unhandled exception occurred while processing channel {channel.Id}: {ex.Message}");

                // Since we hit an error, we should probably stop processing this channel
                // to avoid getting stuck in a tight error loop.
                continue;
            }

            if (!foundAny)
            {
                break;
            }
        }
    }

    private static ulong DateTimeToSnowflake(DateTimeOffset dateTime)
    {
        const long discordEpoch = 1420070400000;
        var milliseconds = dateTime.ToUnixTimeMilliseconds() - discordEpoch;

        return (ulong)milliseconds << 22;
    }
}