using IchigoHoshimiya.Entities.AniList;
using IchigoHoshimiya.Entities.Animethemes;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Context;

public class IchigoContext : DbContext
{
    public IchigoContext()
    {
    }

    public IchigoContext(DbContextOptions<IchigoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiringAnime> AiringAnime { get; set; }
    
    public virtual DbSet<AiringEpisodes> AiringEpisodes { get; set; }
}