using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[PrimaryKey("StudioId", "ImageId")]
[Table("studio_image")]
[Index("ImageId", Name = "studio_image_image_id_foreign")]
public partial class StudioImage
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key]
    [Column("studio_id")]
    public ulong StudioId { get; set; }

    [Key]
    [Column("image_id")]
    public ulong ImageId { get; set; }

    [ForeignKey("ImageId")]
    [InverseProperty("StudioImages")]
    public virtual Image Image { get; set; } = null!;

    [ForeignKey("StudioId")]
    [InverseProperty("StudioImages")]
    public virtual Studio Studio { get; set; } = null!;
}
