using IchigoHoshimiya.Interfaces;
using NetCord.Rest;

namespace IchigoHoshimiya.Adapters;

public class RestClientAdapter(RestClient restClient) : IClient
{
    public Task SendMessageAsync(ulong channelId, string content)
    {
        return restClient.SendMessageAsync(channelId, content);
    }

    // Will be useful at some point I bet
    public Task SendEmbedMessageAsync(ulong channelId, MessageProperties messageProperties)
    {
        return restClient.SendMessageAsync(channelId, messageProperties);
    }

    public Task DeleteMessageAsync(ulong channelId, ulong messageId)
    {
        return restClient.DeleteMessageAsync(channelId, messageId);
    }
}