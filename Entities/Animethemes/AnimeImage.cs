using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities.Animethemes;

[PrimaryKey("AnimeId", "ImageId")]
[Table("anime_image")]
[Index("ImageId", Name = "anime_image_image_id_foreign")]
public class AnimeImage
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key] [Column("anime_id")] public ulong AnimeId { get; set; }

    [Key] [Column("image_id")] public ulong ImageId { get; set; }

    [ForeignKey("AnimeId")]
    [InverseProperty("AnimeImages")]
    public virtual Anime Anime { get; set; } = null!;

    [ForeignKey("ImageId")]
    [InverseProperty("AnimeImages")]
    public virtual Image Image { get; set; } = null!;
}