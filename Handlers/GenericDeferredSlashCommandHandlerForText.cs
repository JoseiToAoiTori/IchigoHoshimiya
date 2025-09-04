using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IchigoHoshimiya.Handlers;

public class GenericDeferredSlashCommandHandlerForText(
    ApplicationCommandContext context,
    Func<Task<string>> contentFunction)
    : IDeferredCommand
{
    public async Task ExecuteAsync()
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

        var content = await contentFunction();

        await context.Interaction.ModifyResponseAsync(message => message.WithContent(content));
    }
}