using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.Commands;

namespace IchigoHoshimiya.Modules;

[UsedImplicitly]
public class SingleThemeTextCommandModule(IAnimethemeService animethemeService) : CommandModule<CommandContext>
{
    [Command("theme")]
    [UsedImplicitly]
    public string GetAnimetheme([CommandParameter(Remainder = true)] string query)
    {
        return animethemeService.GetAnimetheme(query);
    }
}