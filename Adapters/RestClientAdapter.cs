using IchigoHoshimiya.Interfaces;
using NetCord.Rest;

namespace IchigoHoshimiya.Adapters;

public class RestClientAdapter(RestClient restClient) : IClient
{
    public Task SendMessageAsync(ulong channelId, string content)
    {
        return restClient.SendMessageAsync(channelId, content);
    }

    public Task DeleteMessageAsync(ulong channelId, ulong messageId)
    {
        return restClient.DeleteMessageAsync(channelId, messageId);
    }
}