using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace IchigoHoshimiya.Handlers;

[UsedImplicitly]
public class MessageReactionHandler(IClient client) : IMessageReactionAddGatewayHandler
{
    public ValueTask HandleAsync(MessageReactionAddEventArgs arg)
    {
        HandleTwitterDelete(arg);

        return ValueTask.CompletedTask;
    }

    private async void HandleTwitterDelete(MessageReactionAddEventArgs arg)
    {
        try
        {
            if (arg.Emoji.Name != "✂️")
            {
                return;
            }

            var message = await client.GetMessageAsync(arg.ChannelId, arg.MessageId);

            var reactor = arg.User;

            if (message.Content.StartsWith($"Sent by {reactor!.Username}:", StringComparison.OrdinalIgnoreCase))
            {
                await message.DeleteAsync();
            }
        }
        catch (RestException ex)
        {
            Console.WriteLine($"Error while trying to delete message {arg.MessageId}: {ex.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while trying to delete message {arg.MessageId}: {e.Message}");
        }
    }
}