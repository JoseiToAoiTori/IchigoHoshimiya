using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IchigoHoshimiya.Entities.AniList;

[Table("airing_anime")]
public class AiringAnime
{
    [Key] [Column("id")] public long Id { get; init; }

    [Column("anime_title")]
    [MaxLength(10000)]
    public required string Title{ get; set; }

    [Column("anilist_id")] public long AnilistId { get; init; }

    public ICollection<AiringEpisode> AiringEpisodes { get; init; } =  new List<AiringEpisode>();
}