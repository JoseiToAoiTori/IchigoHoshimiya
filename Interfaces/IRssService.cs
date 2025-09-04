namespace IchigoHoshimiya.Interfaces;

public interface IRssService
{
    public Task<string> CreateRssReminder(string query, string? mentions, long channelId, long createdById);
}