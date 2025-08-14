using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[PrimaryKey("ArtistId", "ResourceId")]
[Table("artist_resource")]
[Index("ResourceId", Name = "artist_resource_resource_id_foreign")]
public partial class ArtistResource
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key]
    [Column("artist_id")]
    public ulong ArtistId { get; set; }

    [Key]
    [Column("resource_id")]
    public ulong ResourceId { get; set; }

    [Column("as")]
    [StringLength(255)]
    public string? As { get; set; }

    [ForeignKey("ArtistId")]
    [InverseProperty("ArtistResources")]
    public virtual Artist Artist { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("ArtistResources")]
    public virtual Resource Resource { get; set; } = null!;
}
