using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities.Animethemes;

[PrimaryKey("EntryId", "VideoId")]
[Table("anime_theme_entry_video")]
[Index("VideoId", Name = "anime_theme_entry_video_video_id_foreign")]
public class AnimeThemeEntryVideo
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key] [Column("entry_id")] public ulong EntryId { get; set; }

    [Key] [Column("video_id")] public ulong VideoId { get; set; }

    [ForeignKey("EntryId")]
    [InverseProperty("AnimeThemeEntryVideos")]
    public virtual AnimeThemeEntry Entry { get; set; } = null!;

    [ForeignKey("VideoId")]
    [InverseProperty("AnimeThemeEntryVideos")]
    public virtual Video Video { get; set; } = null!;
}