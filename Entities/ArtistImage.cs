using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[PrimaryKey("ArtistId", "ImageId")]
[Table("artist_image")]
[Index("ImageId", Name = "artist_image_image_id_foreign")]
public partial class ArtistImage
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key]
    [Column("artist_id")]
    public ulong ArtistId { get; set; }

    [Key]
    [Column("image_id")]
    public ulong ImageId { get; set; }

    [Column("depth")]
    public int? Depth { get; set; }

    [ForeignKey("ArtistId")]
    [InverseProperty("ArtistImages")]
    public virtual Artist Artist { get; set; } = null!;

    [ForeignKey("ImageId")]
    [InverseProperty("ArtistImages")]
    public virtual Image Image { get; set; } = null!;
}
