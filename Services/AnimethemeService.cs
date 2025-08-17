using System.Text;
using FuzzySharp;
using IchigoHoshimiya.Context;
using IchigoHoshimiya.DTO;
using IchigoHoshimiya.Entities;
using IchigoHoshimiya.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCord;
using NetCord.Rest;

namespace IchigoHoshimiya.Services;

public class AnimethemeService(AnimethemesDbContext dbContext, IConfiguration configuration) : IAnimethemeService
{
    public EmbedProperties GetAllAnimethemes(string query, string? slug)
    {
        List<AnimethemeDto> fuzzyMatchesDto = GetFuzzyMatches(query, 10, slug);

        if (fuzzyMatchesDto.Count == 0)
        {
            return new EmbedProperties()
               .WithTitle("No matches found for your query");
        }

        var embedDescriptionBuilder = new StringBuilder();

        for (var i = 0; i < fuzzyMatchesDto.Count; i++)
        {
            AnimethemeDto match = fuzzyMatchesDto[i];

            embedDescriptionBuilder.AppendLine(
                $"{i + 1}. [{match.Anime} {match.Slug} - {match.Theme}]({match.Link})"
            );
        }

        return new EmbedProperties()
              .WithTitle("Your search results")
              .WithColor(
                   new Color(
                       (byte)short.Parse(configuration["EmbedColours:Red"]!),
                       (byte)short.Parse(configuration["EmbedColours:Green"]!),
                       (byte)short.Parse(configuration["EmbedColours:Blue"]!)))
              .WithDescription(embedDescriptionBuilder.ToString());
    }

    public string GetAnimetheme(string query, string? slug)
    {
        List<AnimethemeDto> fuzzyMatchesDto = GetFuzzyMatches(query, 5, slug);

        if (fuzzyMatchesDto.Count == 0)
        {
            return "No matches found for your query";
        }

        AnimethemeDto firstMatch = fuzzyMatchesDto[0];

        return $"{firstMatch.Anime} {firstMatch.Slug} - {firstMatch.Theme}\n{firstMatch.Link}";
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

    private List<AnimethemeDto> GetFuzzyMatches(string query, int count, string? slug)
    {
        IQueryable<AnimeThemeEntry> builderQuery = dbContext.AnimeThemeEntries
                                                            .AsSplitQuery()
                                                            .Include(e => e.Theme)
                                                            .ThenInclude(t => t.Song)
                                                            .Include(e => e.Theme)
                                                            .ThenInclude(t => t.Anime)
                                                            .ThenInclude(a => a.AnimeSynonyms)
                                                            .Include(e => e.AnimeThemeEntryVideos)
                                                            .ThenInclude(v => v.Video)
                                                            .Where(e => e.Theme.Song != null)
                                                            .Where(e => e.AnimeThemeEntryVideos.Any());

        if (!string.IsNullOrWhiteSpace(slug))
        {
            builderQuery = builderQuery.Where(e => e.Theme.Slug == slug);
        }

        List<AnimeThemeEntry> candidates = builderQuery.ToList();

        string normalizedQuery = Normalize(query);
        string[] queryTokens = normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bool multiToken = queryTokens.Length > 1;
        string firstToken = queryTokens.FirstOrDefault() ?? normalizedQuery;

        var scored = candidates.AsParallel()
                               .Select(e =>
                                {
                                    string animeName = Normalize(e.Theme.Anime.Name);

                                    List<string> synonyms = e.Theme.Anime.AnimeSynonyms
                                                             .Select(s => Normalize(s.Text ?? ""))
                                                             .ToList();

                                    string themeTitle = Normalize(e.Theme.Song?.Title ?? "");
                                    
                                    int animeNameScore = Math.Max(
                                        Fuzz.TokenSortRatio(normalizedQuery, animeName),
                                        Fuzz.TokenSetRatio(normalizedQuery, animeName)
                                    );

                                    int synonymScore = synonyms.Count > 0
                                        ? synonyms.Max(s => Math.Max(
                                            Fuzz.TokenSortRatio(normalizedQuery, s),
                                            Fuzz.TokenSetRatio(normalizedQuery, s)
                                        ))
                                        : 0;

                                    int bestAnimeScore = Math.Max(animeNameScore, synonymScore);
                                    
                                    int themeScore1 = Fuzz.TokenSetRatio(normalizedQuery, themeTitle);
                                    int themeScore2 = Fuzz.PartialRatio(normalizedQuery, themeTitle);
                                    int bestThemeScore = Math.Max(themeScore1, themeScore2);

                                    // first token vs anime (for anime+theme queries)
                                    int firstTokenAnimeScore = Math.Max(
                                        Fuzz.PartialRatio(firstToken, animeName),
                                        synonyms.Count > 0
                                            ? synonyms.Max(s => Fuzz.PartialRatio(firstToken, s))
                                            : 0
                                    );

                                    // Weighting
                                    double weightedScore = 0.5 * bestThemeScore + 0.5 * bestAnimeScore;

                                    // Boost for exact matches
                                    if (string.Equals(
                                            themeTitle,
                                            normalizedQuery,
                                            StringComparison.OrdinalIgnoreCase) ||
                                        synonyms.Any(s => string.Equals(
                                            s,
                                            normalizedQuery,
                                            StringComparison.OrdinalIgnoreCase)) ||
                                        string.Equals(animeName, normalizedQuery, StringComparison.OrdinalIgnoreCase))
                                    {
                                        weightedScore += 15;
                                    }

                                    // ✅ New: prioritize exact-prefix matches
                                    if (animeName.StartsWith(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                                    {
                                        weightedScore +=
                                            string.IsNullOrEmpty(slug) ? 10 : 20;
                                    }
                                    else if (animeName.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                                    {
                                        weightedScore +=
                                            string.IsNullOrEmpty(slug) ? 5 : 10;
                                    }

                                    return new
                                    {
                                        Entry = e,
                                        Score = weightedScore,
                                        AnimeScore = bestAnimeScore,
                                        ThemeScore = bestThemeScore,
                                        FirstTokenAnimeScore = firstTokenAnimeScore
                                    };
                                })
                               .Where(x =>
                                {
                                    if (!multiToken)
                                    {
                                        return x.AnimeScore >= 80;
                                    }

                                    if (x.FirstTokenAnimeScore >= 60)
                                    {
                                        return (x.ThemeScore >= 65 && x.AnimeScore >= 35) || x.AnimeScore >= 55;
                                    }

                                    return x.ThemeScore >= 70;
                                })
                               .OrderByDescending(x => x.Score)
                               .GroupBy(x => x.Entry.Theme.ThemeId)
                               .Select(g => g.First())
                               .Take(count)
                               .ToList();

        List<AnimeThemeEntry> fuzzyMatches = scored
                                            .OrderByDescending(x => x.Score)
                                            .Take(count)
                                            .Select(x => x.Entry)
                                            .ToList();

        return ToDto(fuzzyMatches);
        
        string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }

            string lowered = input.ToLowerInvariant();
            var noPunct = new string(lowered.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());

            return string.Join(" ", noPunct.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}