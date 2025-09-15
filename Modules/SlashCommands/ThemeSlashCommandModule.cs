using IchigoHoshimiya.Handlers;
using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules.SlashCommands;

[UsedImplicitly]
public class ThemeSlashCommandModule(IAnimethemeService animethemeService)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("themes", "Returns up to 10 anime themes for the specified query.")]
    [UsedImplicitly]
    public async Task GetAnimethemes(
        [SlashCommandParameter(
            Name = "query",
            Description =
                "A search query (anime or theme name)")]
        string query,
        [SlashCommandParameter(Name = "slug", Description = "An optional slug e.g OP2, ED1 etc.")]
        string? slug = "")
    {
        var command = new GenericDeferredSlashCommandHandlerForEmbed(Context, ThemeFunction);

        await command.ExecuteAsync();

        return;

        Task<EmbedProperties> ThemeFunction()
        {
            return Task.Run(() => animethemeService.GetAllAnimethemes(query, slug));
        }
    }

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

        Task<string> ThemeFunction()
        {
            return Task.Run(() => animethemeService.GetAnimetheme(query, slug));
        }
    }
}