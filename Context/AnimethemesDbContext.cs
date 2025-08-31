using IchigoHoshimiya.Entities.Animethemes;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Context;

public class AnimethemesDbContext : DbContext
{
    public AnimethemesDbContext()
    {
    }

    public AnimethemesDbContext(DbContextOptions<AnimethemesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Anime> Animes { get; set; }

    public virtual DbSet<AnimeImage> AnimeImages { get; set; }

    public virtual DbSet<AnimeResource> AnimeResources { get; set; }

    public virtual DbSet<AnimeSeries> AnimeSeries { get; set; }

    public virtual DbSet<AnimeStudio> AnimeStudios { get; set; }

    public virtual DbSet<AnimeSynonym> AnimeSynonyms { get; set; }

    public virtual DbSet<AnimeTheme> AnimeThemes { get; set; }

    public virtual DbSet<AnimeThemeEntry> AnimeThemeEntries { get; set; }

    public virtual DbSet<AnimeThemeEntryVideo> AnimeThemeEntryVideos { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<ArtistImage> ArtistImages { get; set; }

    public virtual DbSet<ArtistMember> ArtistMembers { get; set; }

    public virtual DbSet<ArtistResource> ArtistResources { get; set; }

    public virtual DbSet<ArtistSong> ArtistSongs { get; set; }

    public virtual DbSet<Audio> Audios { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Performance> Performances { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<SongResource> SongResources { get; set; }

    public virtual DbSet<Studio> Studios { get; set; }

    public virtual DbSet<StudioImage> StudioImages { get; set; }

    public virtual DbSet<StudioResource> StudioResources { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    public virtual DbSet<VideoScript> VideoScripts { get; set; }
}