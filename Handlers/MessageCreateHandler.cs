using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace IchigoHoshimiya.Handlers;

public class MessageCreateHandler(RestClient client) : IMessageCreateGatewayHandler
{
    public async ValueTask HandleAsync(Message message)
    {
        if (message.Author.IsBot)
            return;

        string content = message.Content?.Trim() ?? string.Empty;

        switch (content.ToLower())
        {
            case "hello":
                await client.SendMessageAsync(message.ChannelId, "Hello!");
                break;
        }
    }
}