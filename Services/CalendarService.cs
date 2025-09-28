using IchigoHoshimiya.Context;
using IchigoHoshimiya.Helpers;
using IchigoHoshimiya.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetCord.Rest;

namespace IchigoHoshimiya.Services;

public class CalendarService(IchigoContext dbContext) : ICalendarService
{
    public async Task<EmbedProperties> GetCalendar(DateTime targetDate)
    {
        var episodesAiring = await dbContext.AiringEpisodes
                                            .Where(episode => episode.AiringAtUtc.Date == targetDate.Date)
                                            .OrderBy(episode => episode.AiringAtUtc)
                                            .Include(airingEpisode => airingEpisode.Anime!)
                                            .ToListAsync();

        var description = "No episodes airing.";

        if (episodesAiring.Count <= 0)
        {
            return EmbedHelper.Build($"Schedule for {targetDate:dddd} ({targetDate:yyyy-MM-dd}, UTC+0)", description);
        }

        var groupedEpisodes = episodesAiring.GroupBy(e => new { e.AnimeId, e.AiringAtUtc })
                                            .Select(g => g.ToList());

        var descriptionList = (from @group in groupedEpisodes
                               let firstEpisode = @group.First()
                               select @group.Count > 1
                                   ? $"**{firstEpisode.Anime!.Title}**\n{firstEpisode.AiringAtUtc:HH:mm}"
                                   : $"**{firstEpisode.Anime!.Title} {firstEpisode.EpisodeNumber}**\n{firstEpisode.AiringAtUtc:HH:mm}")
           .ToList();

        description = string.Join("\n", descriptionList);

        return EmbedHelper.Build($"Schedule for {targetDate:dddd} ({targetDate:yyyy-MM-dd}, UTC+0)", description);
    }
}