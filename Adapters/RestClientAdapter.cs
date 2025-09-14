using IchigoHoshimiya.Interfaces;
using NetCord.Rest;

namespace IchigoHoshimiya.Adapters;

public class RestClientAdapter(RestClient restClient) : IClient
{
    public Task SendMessageAsync(ulong channelId, string content)
    {
        return restClient.SendMessageAsync(channelId, content);
    }
    
    public Task SendEmbedMessageAsync(ulong channelId, MessageProperties messageProperties)
    {
        return restClient.SendMessageAsync(channelId, messageProperties);
    }
    
    public Task EditEmbedMessageAsync(ulong channelId, ulong messageId, MessageProperties messageProperties)
    {
        return restClient.ModifyMessageAsync(channelId, messageId, options =>
        {
            options.Content = messageProperties.Content;
            options.Embeds = messageProperties.Embeds;
        });
    }

    public Task DeleteMessageAsync(ulong channelId, ulong messageId)
    {
        return restClient.DeleteMessageAsync(channelId, messageId);
    }

    public async Task<RestMessage> GetMessageAsync(ulong channelId, ulong messageId)
    {
        var message = await restClient.GetMessageAsync(channelId, messageId);

        return message;
    }
}