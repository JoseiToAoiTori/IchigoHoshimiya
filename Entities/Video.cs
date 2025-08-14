using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("videos")]
[Index("AudioId", Name = "videos_audio_id_foreign")]
public partial class Video
{
    [Key]
    [Column("video_id")]
    public ulong VideoId { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [Column("basename")]
    [StringLength(255)]
    public string Basename { get; set; } = null!;

    [Column("filename")]
    [StringLength(255)]
    public string Filename { get; set; } = null!;

    [Column("path")]
    [StringLength(255)]
    public string Path { get; set; } = null!;

    [Column("size")]
    public int Size { get; set; }

    [Column("mimetype")]
    [StringLength(255)]
    public string Mimetype { get; set; } = null!;

    [Column("resolution")]
    public int? Resolution { get; set; }

    [Column("nc")]
    public bool Nc { get; set; }

    [Column("subbed")]
    public bool Subbed { get; set; }

    [Column("lyrics")]
    public bool Lyrics { get; set; }

    [Column("uncen")]
    public bool Uncen { get; set; }

    [Column("overlap")]
    public int Overlap { get; set; }

    [Column("source")]
    public int? Source { get; set; }

    [Column("audio_id")]
    public ulong? AudioId { get; set; }

    [InverseProperty("Video")]
    public virtual ICollection<AnimeThemeEntryVideo> AnimeThemeEntryVideos { get; set; } = new List<AnimeThemeEntryVideo>();

    [ForeignKey("AudioId")]
    [InverseProperty("Videos")]
    public virtual Audio? Audio { get; set; }

    [InverseProperty("Video")]
    public virtual ICollection<VideoScript> VideoScripts { get; set; } = new List<VideoScript>();
}
