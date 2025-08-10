using IchigoHoshimiya.Interfaces;
using JetBrains.Annotations;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace IchigoHoshimiya.Handlers;

// sendmessage and delete message and client itself need to be abstracted behind IClient
[UsedImplicitly]
public class MessageCreateHandler(RestClient client, ITwitterReplacementService twitterReplacementService)
    : IMessageCreateGatewayHandler
{
    public ValueTask HandleAsync(Message message)
    {
        if (message.Author.IsBot)
        {
            return ValueTask.CompletedTask;
        }

        // I think discord now has button support for messages, so can we show a button to the user to delete the message?
        HandleTwitter(message);

        return ValueTask.CompletedTask;
    }

    private async void HandleTwitter(Message message)
    {
        try
        {
            string? newContent = await twitterReplacementService.GetReplacedContentAsync(message.Content);

            if (newContent is null)
            {
                return;
            }

            _ = client.SendMessageAsync(message.ChannelId, newContent);
            _ = client.DeleteMessageAsync(message.ChannelId, message.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while sending the message: {e.Message}");
        }
    }
}