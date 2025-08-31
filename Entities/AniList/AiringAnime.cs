using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IchigoHoshimiya.Entities.AniList;

public class AiringAnime
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    [Column("anime_title")]
    public string Title { get; set; }
}