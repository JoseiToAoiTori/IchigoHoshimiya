using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IchigoHoshimiya.Entities.Animethemes;

namespace IchigoHoshimiya.Entities.AniList;

public class AiringEpisodes
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    [Column("episode_number")]
    public int EpisodeNumber { get; set; }
    
    [Column("airing_at_utc")]
    public DateTime AiringAtUtc { get; set; }
    
    [Column("anime_id")]
    public long AnimeId { get; set; }
    
    [ForeignKey("AnimeId")]
    public virtual AiringAnime Anime { get; set; }
}