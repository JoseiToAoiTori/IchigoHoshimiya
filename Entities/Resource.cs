using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("resources")]
public partial class Resource
{
    [Key]
    [Column("resource_id")]
    public ulong ResourceId { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [Column("site")]
    public int? Site { get; set; }

    [Column("link")]
    [StringLength(255)]
    public string? Link { get; set; }

    [Column("external_id")]
    public int? ExternalId { get; set; }

    [InverseProperty("Resource")]
    public virtual ICollection<AnimeResource> AnimeResources { get; set; } = new List<AnimeResource>();

    [InverseProperty("Resource")]
    public virtual ICollection<ArtistResource> ArtistResources { get; set; } = new List<ArtistResource>();

    [InverseProperty("Resource")]
    public virtual ICollection<SongResource> SongResources { get; set; } = new List<SongResource>();

    [InverseProperty("Resource")]
    public virtual ICollection<StudioResource> StudioResources { get; set; } = new List<StudioResource>();
}
