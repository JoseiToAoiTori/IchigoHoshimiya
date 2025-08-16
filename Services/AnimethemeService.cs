using FuzzySharp;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.DTO;
using IchigoHoshimiya.Entities;
using IchigoHoshimiya.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Services;

public class AnimethemeService(AnimethemesDbContext dbContext) : IAnimethemeService
{
    public List<AnimethemeDto>? GetAllAnimethemes(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return null;
        }

        string[] queryTokens = query.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        List<AnimeThemeEntry> candidates = dbContext.AnimeThemeEntries
                                                    .Include(e => e.Theme)
                                                    .ThenInclude(t => t.Song)
                                                    .Include(e => e.Theme)
                                                    .ThenInclude(t => t.Anime)
                                                    .ThenInclude(a => a.AnimeSynonyms)
                                                    .Include(e => e.AnimeThemeEntryVideos)
                                                    .ThenInclude(v => v.Video)
                                                    .Where(e => e.Theme.Song != null)
                                                    .AsEnumerable()
                                                    .Where(e => queryTokens.Any(token =>
                                                         e.Theme.Slug.Contains(
                                                             token,
                                                             StringComparison.CurrentCultureIgnoreCase) ||
                                                         (e.Theme.Song?.Title ?? "").Contains(
                                                             token,
                                                             StringComparison.CurrentCultureIgnoreCase) ||
                                                         e.Theme.Anime.Name.Contains(
                                                             token,
                                                             StringComparison.CurrentCultureIgnoreCase) ||
                                                         e.Theme.Anime.AnimeSynonyms.Any(s => (s.Text ?? "").Contains(
                                                             token,
                                                             StringComparison.CurrentCultureIgnoreCase))
                                                     ))
                                                    .ToList();

        List<AnimeThemeEntry> fuzzyMatches = candidates
                                            .Select(e =>
                                             {
                                                 var totalScore = 0.0;
                                                 var matchedTokens = 0;

                                                 foreach (string token in queryTokens)
                                                 {
                                                     var scoresForToken = new List<int>
                                                     {
                                                         Fuzz.PartialRatio(token, e.Theme.Slug ?? ""),
                                                         Fuzz.PartialRatio(token, e.Theme.Song?.Title ?? ""),
                                                         Fuzz.PartialRatio(token, e.Theme.Anime?.Name ?? "")
                                                     };

                                                     // Match against synonyms
                                                     if (e.Theme.Anime?.AnimeSynonyms != null)
                                                     {
                                                         scoresForToken.AddRange(
                                                             e.Theme.Anime.AnimeSynonyms.Select(synonym =>
                                                                 Fuzz.PartialRatio(token, synonym.Text ?? "")));
                                                     }

                                                     int maxScoreForToken = scoresForToken.Max();

                                                     if (maxScoreForToken < 80)
                                                     {
                                                         continue;
                                                     }

                                                     totalScore += maxScoreForToken;
                                                     matchedTokens++;
                                                 }

                                                 double finalScore = matchedTokens > 0 ? totalScore / matchedTokens : 0;

                                                 return new
                                                 {
                                                     Entry = e,
                                                     Score = finalScore
                                                 };
                                             })
                                            .Take(10)
                                            .Where(x => x.Score >= 80)
                                            .OrderByDescending(x => x.Score)
                                            .Select(x => x.Entry)
                                            .ToList();

        List<AnimethemeDto> fuzzyMatchesDto = ToDto(fuzzyMatches);

        return fuzzyMatchesDto;
    }

    public AnimethemeDto GetAnimetheme(string query)
    {
        throw new NotImplementedException();
    }

    private static List<AnimethemeDto> ToDto(List<AnimeThemeEntry> themes)
    {
        return themes.Select(theme =>
                      {
                          Video? highestResVideo = theme.AnimeThemeEntryVideos
                                                        .Select(entryVideo => entryVideo.Video)
                                                        .OrderByDescending(video => video.Resolution)
                                                        .FirstOrDefault();

                          return new AnimethemeDto
                          {
                              Anime = theme.Theme.Anime.Name,
                              Slug = theme.Theme.Slug,
                              Theme = theme.Theme.Song!.Title!,
                              Link = $"https://v.animethemes.moe/{highestResVideo!.Basename}"
                          };
                      })
                     .ToList();
    }
}