using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules.TextCommands;

public class ChooseTextCommandModule(IChooseService chooseService)
    : CommandModule<CommandContext>
{
    [Command("choose")]
    [UsedImplicitly]
    public string GetChoice([CommandParameter(Remainder = true)] string choices)
    {
        return chooseService.GetRandomChoice(choices.Split(","));
    }
}