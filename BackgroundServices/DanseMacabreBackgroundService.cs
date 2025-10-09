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
        514203145333899278,
        514215337496150018, // anime
        514215404575653899, // screencaps
        514203145333899280,
        514216244598145065,
        514249693228826626,
        514215571731251215,
        514216176029532162,
        514216541940744202,
        514216680201912320,
        514217129873113101,
        514217212446375946,
        514217836122603530,
        514220178511233024,
        514249813089583114,
        514249855938330627,
        514249909512175636,
        514548336011771904,
        514550463493046283,
        514689211975204866,
        524030817501052938,
        544717127853015043,
        544719045430214656,
        544873313995063296,
        546508584117075968,
        551999759079047170,
        562091080410202113,
        563906111565201428,
        563908780094521355,
        563908816970711051,
        563919662144946228,
        567201805675790336,
        581454411340054528,
        582289073713184778,
        584408824367022082,
        585450626192769024,
        586365164379439114,
        590704045153320961,
        591386590824235012,
        599808571500134410,
        601461016664211479,
        609514114951151670,
        612380407719264256,
        613111937995112494,
        620810743956111391,
        625858234627915777,
        681570581094858773,
        689097058560376853,
        698989739797119057,
        702992079948873790,
        729033078202302505,
        754175769265176648,
        782505735392329750,
        811326199510204476,
        816873250717433916,
        817196718520139807,
        820821140250558506,
        828606515241680927,
        832862899587514378,
        877592570195234876,
        945477896460447764,
        945784960395128852,
        949881603562287166,
        966015928636698684,
        967353675976704040,
        973341933944598569,
        1000976676949340250,
        1002199140542992434,
        1006619043677081610,
        1029284353466966137,
        1040069462096887858,
        1079256093529735270,
        1079257329398198272,
        1080325167068229672,
        1083446590833963018,
        1084220163836084296,
        1181490235570409532,
        1203112172306305077,
        1214659790970691634,
        1233152598257827862,
        1332549454791704626,
        1334238909323935824,
        1351628356856053780,
        1351683168348016680,
        1359395731072614562,
        1362814442043080764,
        1396550243482603710
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
                      .Where(c => !_ignoredChannelIds.Contains(c.Id));

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