using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules.TextCommands;

[UsedImplicitly]
public class PingTextCommandModule(IPingService pingService) : CommandModule<CommandContext>
{
    [Command("ping")]
    [UsedImplicitly]
    public string Ping()
    {
        return pingService.Ping();
    }
}