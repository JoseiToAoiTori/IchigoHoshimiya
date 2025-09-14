using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules.SlashCommands;

public class PingSlashCommandModule(IPingService pingService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ping", "Pong!")]
    [UsedImplicitly]
    public string Ping()
    {
        return pingService.Ping();
    }
}