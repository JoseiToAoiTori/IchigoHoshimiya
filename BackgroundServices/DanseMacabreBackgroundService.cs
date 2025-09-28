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
        514369023568510979,
        514223283407683584,
        514215944319401996,
        514221599965052956,
        514203145333899278 // general
    ];

    private readonly DateTimeOffset _lowerBoundDate = new(2022, 2, 19, 0, 0, 0, TimeSpan.Zero);
    private readonly ulong _userId = 291678129586438144;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channels = await restClient.GetGuildChannelsAsync(_guildId, cancellationToken: stoppingToken);

        var filtered = channels
                      .OfType<TextGuildChannel>()
                      .Where(c => !_ignoredChannelIds.Contains(c.Id));

        foreach (var channel in filtered)
        {
            if (channel is not TextGuildChannel textChannel)
            {
                continue;
            }

            await ProcessChannelAsync(textChannel, stoppingToken);
        }
    }

    private async Task ProcessChannelAsync(TextGuildChannel channel, CancellationToken stoppingToken)
    {
        DateTimeOffset? beforeDateTime = _cutoffDate;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (beforeDateTime <= _lowerBoundDate)
            {
                break;
            }

            var pagination = new PaginationProperties<ulong>
            {
                From = DateTimeToSnowflake(beforeDateTime.Value),
                Direction = PaginationDirection.Before,
                BatchSize = 100
            };

            var foundAny = false;

            await foreach (var message in channel.GetMessagesAsync(pagination).WithCancellation(stoppingToken))
            {
                foundAny = true;

                if (message.Author.Id == _userId && message.CreatedAt < _cutoffDate)
                {
                    try
                    {
                        await message.DeleteAsync(cancellationToken: stoppingToken);

                        Console.WriteLine(
                            $"A message at {message.CreatedAt} in channel {message.ChannelId} was deleted");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete {message.Id} in {channel.Id}: {ex.Message}");
                    }
                }

                beforeDateTime = message.CreatedAt;
                // just don't go crazy with it ig
                await Task.Delay(100, stoppingToken);
            }

            if (!foundAny)
            {
                break;
            }
        }
    }


    private static ulong DateTimeToSnowflake(DateTimeOffset dateTime)
    {
        var discordEpoch = new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var milliseconds = (dateTime - discordEpoch).TotalMilliseconds;

        return (ulong)milliseconds << 22;
    }
}