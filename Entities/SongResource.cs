using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[PrimaryKey("SongId", "ResourceId")]
[Table("song_resource")]
[Index("ResourceId", Name = "song_resource_resource_id_foreign")]
public partial class SongResource
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key]
    [Column("song_id")]
    public ulong SongId { get; set; }

    [Key]
    [Column("resource_id")]
    public ulong ResourceId { get; set; }

    [Column("as")]
    [StringLength(255)]
    public string? As { get; set; }

    [ForeignKey("ResourceId")]
    [InverseProperty("SongResources")]
    public virtual Resource Resource { get; set; } = null!;

    [ForeignKey("SongId")]
    [InverseProperty("SongResources")]
    public virtual Song Song { get; set; } = null!;
}
