using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities;

[Table("video_scripts")]
[Index("VideoId", Name = "video_scripts_video_id_foreign")]
public partial class VideoScript
{
    [Key]
    [Column("script_id")]
    public ulong ScriptId { get; set; }

    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamp(6)")]
    public DateTime? DeletedAt { get; set; }

    [Column("path")]
    [StringLength(255)]
    public string Path { get; set; } = null!;

    [Column("video_id")]
    public ulong? VideoId { get; set; }

    [ForeignKey("VideoId")]
    [InverseProperty("VideoScripts")]
    public virtual Video? Video { get; set; }
}
