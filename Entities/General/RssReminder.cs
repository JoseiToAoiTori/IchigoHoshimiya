using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities.General;

[Table("rss_reminder")]
[Index(nameof(CreatedById))]
[Index(nameof(CreatedAtUtc))]
public class RssReminder
{
    [Key] [Column("id")] public long Id { get; init; }

    [Column("search_string")]
    [MaxLength(1000)]
    public string SearchString { get; init; }

    [Column("enabled")] public bool Enabled { get; set; } = true;

    [Column("created_at_utc")] public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    [Column("created_by_id")] public long CreatedById { get; init; }

    [Column("mentions")] public string? Mentions { get; init; }

    [Column("channel_id")] public long ChannelId { get; init; }
}