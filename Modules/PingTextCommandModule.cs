using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules;

[UsedImplicitly]
public class PingTextCommandModule(IPingService pingService) :CommandModule<CommandContext>
{
    [Command("ping")]
    [UsedImplicitly]
    public string Ping() => pingService.Ping();
}