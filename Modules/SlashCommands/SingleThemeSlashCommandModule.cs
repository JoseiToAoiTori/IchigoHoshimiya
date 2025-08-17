using IchigoHoshimiya.Handlers;
using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules.SlashCommands;

[UsedImplicitly]
public class SingleThemeSlashCommandModule(IAnimethemeService animethemeService)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("theme", "Returns a single anime theme for the specified query")]
    [UsedImplicitly]
    public async Task GetAnimetheme(string query, string? slug = "")
    {
        var command = new GenericDeferredSlashCommandHandlerForText(Context, ThemeFunction);

        await command.ExecuteAsync();

        return;

        // Define the function that gets the theme.
        Task<string> ThemeFunction()
        {
            return Task.Run(() => animethemeService.GetAnimetheme(query, slug));
        }
    }
}