using IchigoHoshimiya.Handlers;
using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules.SlashCommands;

[UsedImplicitly]
public class MultiThemeSlashCommandModule(IAnimethemeService animethemeService)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("themes", "Returns up to 10 anime themes for the specified query.")]
    [UsedImplicitly]
    public async Task GetAnimetheme(string query, string? slug = "")
    {
        var command = new GenericDeferredSlashCommandHandlerForEmbed(Context, ThemeFunction);

        await command.ExecuteAsync();

        return;

        // Define the function that gets the themes
        Task<EmbedProperties> ThemeFunction()
        {
            return Task.Run(() => animethemeService.GetAllAnimethemes(query, slug));
        }
    }
}