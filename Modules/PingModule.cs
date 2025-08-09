using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules;

[UsedImplicitly]
public class PingModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly IPingService _pingService;

    public PingModule(IPingService pingService)
    {
        _pingService = pingService;
    }
    
    [SlashCommand("ping", "Pong!")]
    public string Ping() => _pingService.Ping();
}