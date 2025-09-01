using IchigoHoshimiya.Context;
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
                                            .Include(airingEpisode => airingEpisode.Anime!)
                                            .ToListAsync();

        var description = episodesAiring.Count != 0
            ? string.Join(
                "\n",
                episodesAiring.Select(e =>
                    $"**{e.Anime!.Title}**\n{e.AiringAtUtc:HH:mm}"))
            : "No episodes airing.";

        return new EmbedProperties
        {
            Title = $"Schedule for {targetDate:dddd} (UTC+0)",
            Color = new Color(
                (byte)short.Parse(configuration["EmbedColours:Red"]!),
                (byte)short.Parse(configuration["EmbedColours:Green"]!),
                (byte)short.Parse(configuration["EmbedColours:Blue"]!)),
            Description = description
        };
    }
}