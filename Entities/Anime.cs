using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("anime")]
public partial class Anime
{
    [Key]
    [Column("anime_id")]
    public ulong AnimeId { get; set; }

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

    [Column("year")]
    public int? Year { get; set; }

    [Column("season")]
    public int? Season { get; set; }

    [Column("synopsis", TypeName = "text")]
    public string? Synopsis { get; set; }

    [Column("media_format")]
    public int MediaFormat { get; set; }

    [InverseProperty("Anime")]
    public virtual ICollection<AnimeImage> AnimeImages { get; set; } = new List<AnimeImage>();

    [InverseProperty("Anime")]
    public virtual ICollection<AnimeResource> AnimeResources { get; set; } = new List<AnimeResource>();

    [InverseProperty("Anime")]
    public virtual ICollection<AnimeSeries> AnimeSeries { get; set; } = new List<AnimeSeries>();

    [InverseProperty("Anime")]
    public virtual ICollection<AnimeStudio> AnimeStudios { get; set; } = new List<AnimeStudio>();

    [InverseProperty("Anime")]
    public virtual ICollection<AnimeSynonym> AnimeSynonyms { get; set; } = new List<AnimeSynonym>();

    [InverseProperty("Anime")]
    public virtual ICollection<AnimeTheme> AnimeThemes { get; set; } = new List<AnimeTheme>();
}
