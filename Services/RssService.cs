using IchigoHoshimiya.Context;
using IchigoHoshimiya.Entities.General;
using IchigoHoshimiya.Interfaces;

namespace IchigoHoshimiya.Services;

public class RssService(IchigoContext dbContext) : IRssService
{
    public async Task<string> CreateRssReminder(string query, string? mentions, long channelId, long createdById)
    {
        var reminder = new RssReminder
        {
            ChannelId = channelId,
            CreatedById = createdById,
            SearchString = query,
            Mentions = mentions
        };

        await dbContext.RssReminder.AddAsync(reminder);
        await dbContext.SaveChangesAsync();

        return reminder.Id.ToString();
    }
}