using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("songs")]
public partial class Song
{
    [Key]
    [Column("song_id")]
    public ulong SongId { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string? Title { get; set; }

    [InverseProperty("Song")]
    public virtual ICollection<AnimeTheme> AnimeThemes { get; set; } = new List<AnimeTheme>();

    [InverseProperty("Song")]
    public virtual ICollection<ArtistSong> ArtistSongs { get; set; } = new List<ArtistSong>();

    [InverseProperty("Song")]
    public virtual ICollection<Performance> Performances { get; set; } = new List<Performance>();

    [InverseProperty("Song")]
    public virtual ICollection<SongResource> SongResources { get; set; } = new List<SongResource>();
}
