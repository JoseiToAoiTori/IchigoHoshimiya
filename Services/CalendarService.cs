using IchigoHoshimiya.Context;
using IchigoHoshimiya.Helpers;
using IchigoHoshimiya.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCord;
using NetCord.Rest;

namespace IchigoHoshimiya.Services;

public class CalendarService(IchigoContext dbContext, IConfiguration configuration) : ICalendarService
{
    public async Task<EmbedProperties> GetCalendar(DayOfWeek? dayOfWeek)
    {
        var dateTimeNowUtc = DateTime.UtcNow;
        DateTime targetDate;

        if (dayOfWeek == null)
        {
            targetDate = dateTimeNowUtc.Date;
        }
        else if (dayOfWeek.Value == dateTimeNowUtc.DayOfWeek)
        {
            // If for some reason, someone does /calendar <today>
            targetDate = dateTimeNowUtc.Date;
        }
        else
        {
            var daysUntilTarget = ((int)dayOfWeek.Value - (int)dateTimeNowUtc.DayOfWeek + 7) % 7;

            if (daysUntilTarget == 0)
            {
                daysUntilTarget = 7; // ensure it's NEXT week if same day
            }

            targetDate = dateTimeNowUtc.Date.AddDays(daysUntilTarget);
        }

        var episodesAiring = await dbContext.AiringEpisodes
                                            .Where(episode => episode.AiringAtUtc.Date == targetDate)
                                            .OrderBy(episode => episode.AiringAtUtc)
                                            .Include(airingEpisode => airingEpisode.Anime!)
                                            .ToListAsync();

        var description = "No episodes airing.";

        if (episodesAiring.Count <= 0)
        {
            return EmbedHelper.Build($"Schedule for {targetDate:dddd} (UTC+0)", description);
        }

        var groupedEpisodes = episodesAiring.GroupBy(e => new { e.AnimeId, e.AiringAtUtc })
                                            .Select(g => g.ToList());

        // Pokemon Concierge
        var descriptionList = (from @group in groupedEpisodes let firstEpisode = @group.First() select @group.Count > 1
                                   ? $"**{firstEpisode.Anime!.Title}**\n{firstEpisode.AiringAtUtc:HH:mm}"
                                   : $"**{firstEpisode.Anime!.Title} {firstEpisode.EpisodeNumber}**\n{firstEpisode.AiringAtUtc:HH:mm}")
           .ToList();

        description = string.Join("\n", descriptionList);
        
        return EmbedHelper.Build($"Schedule for {targetDate:dddd} (UTC+0)", description);
    }
}