using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("studios")]
public partial class Studio
{
    [Key]
    [Column("studio_id")]
    public ulong StudioId { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [Column("slug")]
    [StringLength(255)]
    public string Slug { get; set; } = null!;

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [InverseProperty("Studio")]
    public virtual ICollection<AnimeStudio> AnimeStudios { get; set; } = new List<AnimeStudio>();

    [InverseProperty("Studio")]
    public virtual ICollection<StudioImage> StudioImages { get; set; } = new List<StudioImage>();

    [InverseProperty("Studio")]
    public virtual ICollection<StudioResource> StudioResources { get; set; } = new List<StudioResource>();
}
