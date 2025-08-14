using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("audios")]
public partial class Audio
{
    [Key]
    [Column("audio_id")]
    public ulong AudioId { get; set; }

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

    [InverseProperty("Audio")]
    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
