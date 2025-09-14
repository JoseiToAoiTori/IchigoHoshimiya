using NetCord.Rest;

namespace IchigoHoshimiya.Interfaces;

public interface IClient
{
    Task SendMessageAsync(ulong channelId, string content);

    Task SendEmbedMessageAsync(ulong channelId, MessageProperties messageProperties);

    Task EditEmbedMessageAsync(ulong channelId, ulong messageId, MessageProperties messageProperties);

    Task DeleteMessageAsync(ulong channelId, ulong messageId);

    Task<RestMessage> GetMessageAsync(ulong channelId, ulong messageId);
}