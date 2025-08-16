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
    public EmbedProperties GetAllAnimethemes(string query)
    {
        List<AnimethemeDto> fuzzyMatchesDto = GetFuzzyMatches(query, 10);

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

    public string GetAnimetheme(string query)
    {
        List<AnimethemeDto> fuzzyMatchesDto = GetFuzzyMatches(query, 5);

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

    private List<AnimethemeDto> GetFuzzyMatches(string query, int count)
    {
        List<AnimeThemeEntry> candidates = dbContext.AnimeThemeEntries
                                                    .Include(e => e.Theme)
                                                    .ThenInclude(t => t.Song)
                                                    .Include(e => e.Theme)
                                                    .ThenInclude(t => t.Anime)
                                                    .ThenInclude(a => a.AnimeSynonyms)
                                                    .Include(e => e.AnimeThemeEntryVideos)
                                                    .ThenInclude(v => v.Video)
                                                    .Where(e => e.Theme.Song != null)
                                                    .ToList();

        var scored = candidates.Select(e =>
        {
            string animeName = e.Theme.Anime.Name;
            List<string?> synonyms = e.Theme.Anime.AnimeSynonyms.Select(s => s.Text).ToList();

            int exactBoost = animeName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                             synonyms.Any(s => s != null && s.Contains(query, StringComparison.OrdinalIgnoreCase))
                ? 100
                : 0;

            int nameScore = Fuzz.TokenSetRatio(query, animeName);
            int synonymScore = synonyms.Count != 0 ? synonyms.Max(s => Fuzz.TokenSetRatio(query, s)) : 0;

            var themeTitle = $"{e.Theme.Slug} {e.Theme.Song!.Title}";
            int themeScore = Fuzz.PartialRatio(query, themeTitle);

            int finalScore = Math.Max(nameScore, synonymScore) + exactBoost;
            finalScore = Math.Max(finalScore, themeScore / 2);

            return new { Entry = e, Score = finalScore };
        });

        List<AnimeThemeEntry> fuzzyMatches = scored
                                            .OrderByDescending(x => x.Score)
                                            .Take(count)
                                            .Select(x => x.Entry)
                                            .ToList();

        return ToDto(fuzzyMatches);
    }
}