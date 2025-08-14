using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[PrimaryKey("AnimeId", "StudioId")]
[Table("anime_studio")]
[Index("StudioId", Name = "anime_studio_studio_id_foreign")]
public partial class AnimeStudio
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key]
    [Column("anime_id")]
    public ulong AnimeId { get; set; }

    [Key]
    [Column("studio_id")]
    public ulong StudioId { get; set; }

    [ForeignKey("AnimeId")]
    [InverseProperty("AnimeStudios")]
    public virtual Anime Anime { get; set; } = null!;

    [ForeignKey("StudioId")]
    [InverseProperty("AnimeStudios")]
    public virtual Studio Studio { get; set; } = null!;
}
