using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IchigoHoshimiya.Entities.General;

public class GrassToucher
{
    [Key] [Column("id")] public long Id { get; set; }

    [Column("user_id")] public ulong UserId { get; set; }

    [Column("channel_id")] public ulong ChannelId { get; set; }

    [Column("touching_grass_until_utc")] public DateTime TouchingGrassUntilUtc { get; set; }
}