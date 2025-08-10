namespace IchigoHoshimiya.Interfaces;

public interface IClient
{
    Task SendMessageAsync(ulong channelId, string content);
    Task DeleteMessageAsync(ulong channelId, ulong messageId);
}