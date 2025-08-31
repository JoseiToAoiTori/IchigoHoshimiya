using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Entities.Animethemes;

[PrimaryKey("ArtistId", "SongId")]
[Table("artist_song")]
[Index("SongId", Name = "artist_song_song_id_foreign")]
public class ArtistSong
{
    [Column("created_at", TypeName = "timestamp(6)")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp(6)")]
    public DateTime? UpdatedAt { get; set; }

    [Key] [Column("artist_id")] public ulong ArtistId { get; set; }

    [Key] [Column("song_id")] public ulong SongId { get; set; }

    [Column("as")] [StringLength(255)] public string? As { get; set; }

    [Column("alias")] [StringLength(255)] public string? Alias { get; set; }

    [ForeignKey("ArtistId")]
    [InverseProperty("ArtistSongs")]
    public virtual Artist Artist { get; set; } = null!;

    [ForeignKey("SongId")]
    [InverseProperty("ArtistSongs")]
    public virtual Song Song { get; set; } = null!;
}