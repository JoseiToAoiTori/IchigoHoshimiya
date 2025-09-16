using System.Collections.Concurrent;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.Entities.General;
using IchigoHoshimiya.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IchigoHoshimiya.BackgroundServices;

public class GrassToucherReleaserService(
    IServiceProvider serviceProvider,
    ILogger<GrassToucherReleaserService> logger,
    IConfiguration configuration,
    IClient client)
    : BackgroundService
{
    private readonly ConcurrentDictionary<long, Task> _scheduledTasks = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var isRefugeeBot = configuration.GetValue<bool>("RefugeeBot");

        if (isRefugeeBot)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IchigoContext>();

            var now = DateTime.UtcNow;

            var pending = await db.GrassToucher
                                  .Where(g => g.TouchingGrassUntilUtc < now)
                                  .ToListAsync(stoppingToken);

            // On start, release any grass touchers if they have served their time
            foreach (var toucher in pending)
            {
                await ReleaseOne(toucher, stoppingToken);
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }

    public void ScheduleRelease(GrassToucher toucher, CancellationToken stoppingToken)
    {
        var delay = toucher.TouchingGrassUntilUtc - DateTime.UtcNow;

        if (delay < TimeSpan.Zero)
        {
            delay = TimeSpan.Zero;
        }

        var task = Task.Run(
            async () =>
            {
                try
                {
                    await Task.Delay(delay, stoppingToken);

                    await ReleaseOne(toucher, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    _scheduledTasks.TryRemove(toucher.Id, out _);
                }
            },
            stoppingToken);

        _scheduledTasks[toucher.Id] = task;
    }

    private async Task ReleaseOne(GrassToucher toucher, CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IchigoContext>();

            await client.RemoveRoleFromUser(
                configuration.GetValue<ulong>("TouchingGrassGuild"),
                toucher.UserId,
                configuration.GetValue<ulong>("TouchingGrassRole"));

            db.GrassToucher.Remove(toucher);
            await db.SaveChangesAsync(stoppingToken);

            await client.SendMessageAsync(toucher.ChannelId, $"<@{toucher.UserId}> has stopped touching grass.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to release GrassToucher {Id}", toucher.Id);
        }
    }
}