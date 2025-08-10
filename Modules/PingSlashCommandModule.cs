using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules;

[UsedImplicitly]
public class PingSlashCommandModule(IPingService pingService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ping", "Pong!")]
    public string Ping() => pingService.Ping();
}