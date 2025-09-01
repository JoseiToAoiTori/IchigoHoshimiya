using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities.AniList;

[Table("airing_episode")]
[Index(nameof(AiringAtUtc))]
public class AiringEpisode
{
    [Key] [Column("id")] public long Id { get; init; }

    [Column("episode_number")] public int EpisodeNumber { get; init; }

    [Column("airing_at_utc")] public DateTime AiringAtUtc { get; set; }

    [Column("anime_id")] public long AnimeId { get; init; }

    [ForeignKey("AnimeId")] public virtual AiringAnime? Anime { get; init; }
}