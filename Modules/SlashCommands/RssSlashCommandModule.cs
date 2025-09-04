using IchigoHoshimiya.Handlers;
using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Modules.SlashCommands;

public class RssSlashCommandModule(IRssService rssService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("notify", "Notify when a movie/OVA/batch anime drops on nyaa")]
    [UsedImplicitly]
    public async Task CreateRssReminder(
        [SlashCommandParameter(
            Name = "query",
            Description =
                "The name of the movie to search (ideally romaji)")]
        string query,
        [SlashCommandParameter(
            Name = "mentions",
            Description = "Ping people you would like to be mentioned when the movie drops (in addition to yourself)")]
        string? mentions = "")
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

        // 2️⃣ Do the async work
        var id = await rssService.CreateRssReminder(
            query,
            mentions,
            (long)Context.Channel.Id,
            (long)Context.User.Id);

        // 3️⃣ Send the final response
        var safeQuery = query.Replace("`", "");
        await Context.Interaction.ModifyResponseAsync(message =>
            message.WithContent($"✅ Reminder created with Id `{id}` for query **{safeQuery}**"));
    }
}