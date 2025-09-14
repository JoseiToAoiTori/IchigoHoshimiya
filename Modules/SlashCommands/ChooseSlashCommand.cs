using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules.SlashCommands;

public class ChooseSlashCommand(IChooseService chooseService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("choose", "Choose between one of many options, separated by commas")]
    [UsedImplicitly]
    public string GetChoice([CommandParameter(Remainder = true)] string choices)
    {
        return chooseService.GetRandomChoice(choices.Split(","));
    }
}