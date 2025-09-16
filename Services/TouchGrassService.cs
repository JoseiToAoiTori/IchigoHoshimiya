using IchigoHoshimiya.BackgroundServices;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.Entities.General;
using IchigoHoshimiya.Interfaces;
using Microsoft.Extensions.Configuration;

namespace IchigoHoshimiya.Services;

public class TouchGrassService(
    IClient client,
    IConfiguration configuration,
    IchigoContext context,
    GrassToucherReleaserService grassToucherReleaserService) : ITouchGrassService
{
    public async Task AddGrassToucher(ulong guildId, ulong channelId, ulong userId, string period)
    {
        if (!double.TryParse(period, out var hours))
        {
            throw new ArgumentException("Invalid period format. Must be a number.", nameof(period));
        }

        if (hours < 0.02)
        {
            throw new ArgumentOutOfRangeException(nameof(period), "Period must be at least 0.02 hours (~72 seconds).");
        }

        var until = DateTime.UtcNow.AddHours(hours);

        var grassToucher = new GrassToucher
        {
            UserId = userId,
            ChannelId = channelId,
            TouchingGrassUntilUtc = until
        };

        await context.GrassToucher.AddAsync(grassToucher);

        await context.SaveChangesAsync();

        await client.AddRoleToUser(
            guildId,
            userId,
            configuration.GetValue<ulong>("TouchingGrassRole")
        );

        grassToucherReleaserService.ScheduleRelease(grassToucher, CancellationToken.None);
    }
}