using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities.Animethemes;

[PrimaryKey("AnimeId", "SeriesId")]
[Table("anime_series")]
[Index("SeriesId", Name = "anime_series_series_id_foreign")]
public class AnimeSeries
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key] [Column("anime_id")] public ulong AnimeId { get; set; }

    [Key] [Column("series_id")] public ulong SeriesId { get; set; }

    [ForeignKey("AnimeId")]
    [InverseProperty("AnimeSeries")]
    public virtual Anime Anime { get; set; } = null!;

    [ForeignKey("SeriesId")]
    [InverseProperty("AnimeSeries")]
    public virtual Series Series { get; set; } = null!;
}