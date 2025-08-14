using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("anime_synonyms")]
[Index("AnimeId", Name = "anime_synonyms_anime_id_foreign")]
public partial class AnimeSynonym
{
    [Key]
    [Column("synonym_id")]
    public ulong SynonymId { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [Column("text")]
    [StringLength(255)]
    public string? Text { get; set; }

    [Column("anime_id")]
    public ulong AnimeId { get; set; }

    [Column("type")]
    public int Type { get; set; }

    [ForeignKey("AnimeId")]
    [InverseProperty("AnimeSynonyms")]
    public virtual Anime Anime { get; set; } = null!;
}
