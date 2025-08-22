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
    public async Task GetAnimetheme(
        [SlashCommandParameter(
            Name = "query",
            Description =
                "A search query (anime or theme name)")]
        string query,
        [SlashCommandParameter(Name = "slug", Description = "An optional slug e.g OP2, ED1 etc.")]
        string? slug = "")
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