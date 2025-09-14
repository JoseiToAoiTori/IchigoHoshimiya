using JetBrains.Annotations;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules.TextCommands;

public class CountdownModule : CommandModule<CommandContext>
{
    [Command("cd")]
    [UsedImplicitly]
    public async Task Countdown()
    {
        await Context.Message.SendAsync("5");

        for (var i = 4; i >= 1; i--)
        {
            await Task.Delay(1000);

            await Context.Message.SendAsync($"{i}");
        }
    }
}