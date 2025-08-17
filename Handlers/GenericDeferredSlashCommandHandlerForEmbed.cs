using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Handlers;

public class GenericDeferredSlashCommandHandlerForEmbed(
    ApplicationCommandContext context,
    Func<Task<EmbedProperties>> contentFunction)
    : IDeferredCommand
{
    public async Task ExecuteAsync()
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

        EmbedProperties embed = await contentFunction();

        MessageProperties props = new MessageProperties()
           .WithEmbeds([embed]);

        await context.Interaction.ModifyResponseAsync(message => message.Embeds = [embed]);
    }
}