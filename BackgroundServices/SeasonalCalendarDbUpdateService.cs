using Microsoft.Extensions.Hosting;

namespace IchigoHoshimiya.BackgroundServices;

public class SeasonalCalendarDbUpdateService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}