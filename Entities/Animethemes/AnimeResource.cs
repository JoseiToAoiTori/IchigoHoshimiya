using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities.Animethemes;

[PrimaryKey("AnimeId", "ResourceId")]
[Table("anime_resource")]
[Index("ResourceId", Name = "anime_resource_resource_id_foreign")]
public class AnimeResource
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key] [Column("anime_id")] public ulong AnimeId { get; set; }

    [Key] [Column("resource_id")] public ulong ResourceId { get; set; }

    [Column("as")] [StringLength(255)] public string? As { get; set; }

    [ForeignKey("AnimeId")]
    [InverseProperty("AnimeResources")]
    public virtual Anime Anime { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("AnimeResources")]
    public virtual Resource Resource { get; set; } = null!;
}