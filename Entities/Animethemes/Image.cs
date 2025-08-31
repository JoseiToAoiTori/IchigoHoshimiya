﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IchigoHoshimiya.Entities.Animethemes;

[Table("images")]
public class Image
{
    [Key] [Column("image_id")] public ulong ImageId { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [Column("path")] [StringLength(255)] public string Path { get; set; } = null!;

    [Column("facet")] public int? Facet { get; set; }

    [InverseProperty("Image")]
    public virtual ICollection<AnimeImage> AnimeImages { get; set; } = new List<AnimeImage>();

    [InverseProperty("Image")]
    public virtual ICollection<ArtistImage> ArtistImages { get; set; } = new List<ArtistImage>();

    [InverseProperty("Image")]
    public virtual ICollection<StudioImage> StudioImages { get; set; } = new List<StudioImage>();
}