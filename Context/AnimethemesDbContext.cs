using System;
using System.Collections.Generic;
using IchigoHoshimiya.Entities;
using Microsoft.EntityFrameworkCore;

namespace IchigoHoshimiya.Context;

public partial class AnimethemesDbContext : DbContext
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("Server=localhost;Port=3306;Database=animethemes;Uid=root;Pwd=1234;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anime>(entity =>
        {
            entity.HasKey(e => e.AnimeId).HasName("PRIMARY");
        });

        modelBuilder.Entity<AnimeImage>(entity =>
        {
            entity.HasKey(e => new { e.AnimeId, e.ImageId }).HasName("PRIMARY");

            entity.HasOne(d => d.Anime).WithMany(p => p.AnimeImages).HasConstraintName("anime_image_anime_id_foreign");

            entity.HasOne(d => d.Image).WithMany(p => p.AnimeImages).HasConstraintName("anime_image_image_id_foreign");
        });

        modelBuilder.Entity<AnimeResource>(entity =>
        {
            entity.HasKey(e => new { e.AnimeId, e.ResourceId }).HasName("PRIMARY");

            entity.HasOne(d => d.Anime).WithMany(p => p.AnimeResources).HasConstraintName("anime_resource_anime_id_foreign");

            entity.HasOne(d => d.Resource).WithMany(p => p.AnimeResources).HasConstraintName("anime_resource_resource_id_foreign");
        });

        modelBuilder.Entity<AnimeSeries>(entity =>
        {
            entity.HasKey(e => new { e.AnimeId, e.SeriesId }).HasName("PRIMARY");

            entity.HasOne(d => d.Anime).WithMany(p => p.AnimeSeries).HasConstraintName("anime_series_anime_id_foreign");

            entity.HasOne(d => d.Series).WithMany(p => p.AnimeSeries).HasConstraintName("anime_series_series_id_foreign");
        });

        modelBuilder.Entity<AnimeStudio>(entity =>
        {
            entity.HasKey(e => new { e.AnimeId, e.StudioId }).HasName("PRIMARY");

            entity.HasOne(d => d.Anime).WithMany(p => p.AnimeStudios).HasConstraintName("anime_studio_anime_id_foreign");

            entity.HasOne(d => d.Studio).WithMany(p => p.AnimeStudios).HasConstraintName("anime_studio_studio_id_foreign");
        });

        modelBuilder.Entity<AnimeSynonym>(entity =>
        {
            entity.HasKey(e => e.SynonymId).HasName("PRIMARY");

            entity.HasOne(d => d.Anime).WithMany(p => p.AnimeSynonyms).HasConstraintName("anime_synonyms_anime_id_foreign");
        });

        modelBuilder.Entity<AnimeTheme>(entity =>
        {
            entity.HasKey(e => e.ThemeId).HasName("PRIMARY");

            entity.HasOne(d => d.Anime).WithMany(p => p.AnimeThemes).HasConstraintName("anime_themes_anime_id_foreign");

            entity.HasOne(d => d.Group).WithMany(p => p.AnimeThemes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("anime_themes_group_id_foreign");

            entity.HasOne(d => d.Song).WithMany(p => p.AnimeThemes)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("anime_themes_song_id_foreign");
        });

        modelBuilder.Entity<AnimeThemeEntry>(entity =>
        {
            entity.HasKey(e => e.EntryId).HasName("PRIMARY");

            entity.HasOne(d => d.Theme).WithMany(p => p.AnimeThemeEntries).HasConstraintName("anime_theme_entries_theme_id_foreign");
        });

        modelBuilder.Entity<AnimeThemeEntryVideo>(entity =>
        {
            entity.HasKey(e => new { e.EntryId, e.VideoId }).HasName("PRIMARY");

            entity.HasOne(d => d.Entry).WithMany(p => p.AnimeThemeEntryVideos).HasConstraintName("anime_theme_entry_video_entry_id_foreign");

            entity.HasOne(d => d.Video).WithMany(p => p.AnimeThemeEntryVideos).HasConstraintName("anime_theme_entry_video_video_id_foreign");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("PRIMARY");
        });

        modelBuilder.Entity<ArtistImage>(entity =>
        {
            entity.HasKey(e => new { e.ArtistId, e.ImageId }).HasName("PRIMARY");

            entity.HasOne(d => d.Artist).WithMany(p => p.ArtistImages).HasConstraintName("artist_image_artist_id_foreign");

            entity.HasOne(d => d.Image).WithMany(p => p.ArtistImages).HasConstraintName("artist_image_image_id_foreign");
        });

        modelBuilder.Entity<ArtistMember>(entity =>
        {
            entity.HasKey(e => new { e.ArtistId, e.MemberId }).HasName("PRIMARY");

            entity.HasOne(d => d.Artist).WithMany(p => p.ArtistMemberArtists).HasConstraintName("artist_member_artist_id_foreign");

            entity.HasOne(d => d.Member).WithMany(p => p.ArtistMemberMembers).HasConstraintName("artist_member_member_id_foreign");
        });

        modelBuilder.Entity<ArtistResource>(entity =>
        {
            entity.HasKey(e => new { e.ArtistId, e.ResourceId }).HasName("PRIMARY");

            entity.HasOne(d => d.Artist).WithMany(p => p.ArtistResources).HasConstraintName("artist_resource_artist_id_foreign");

            entity.HasOne(d => d.Resource).WithMany(p => p.ArtistResources).HasConstraintName("artist_resource_resource_id_foreign");
        });

        modelBuilder.Entity<ArtistSong>(entity =>
        {
            entity.HasKey(e => new { e.ArtistId, e.SongId }).HasName("PRIMARY");

            entity.HasOne(d => d.Artist).WithMany(p => p.ArtistSongs).HasConstraintName("artist_song_artist_id_foreign");

            entity.HasOne(d => d.Song).WithMany(p => p.ArtistSongs).HasConstraintName("artist_song_song_id_foreign");
        });

        modelBuilder.Entity<Audio>(entity =>
        {
            entity.HasKey(e => e.AudioId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("PRIMARY");

            entity.HasOne(d => d.Artist).WithMany(p => p.MembershipArtists).HasConstraintName("memberships_artist_id_foreign");

            entity.HasOne(d => d.Member).WithMany(p => p.MembershipMembers).HasConstraintName("memberships_member_id_foreign");
        });

        modelBuilder.Entity<Performance>(entity =>
        {
            entity.HasKey(e => e.PerformanceId).HasName("PRIMARY");

            entity.HasOne(d => d.Song).WithMany(p => p.Performances).HasConstraintName("performances_song_id_foreign");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Series>(entity =>
        {
            entity.HasKey(e => e.SeriesId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.SongId).HasName("PRIMARY");
        });

        modelBuilder.Entity<SongResource>(entity =>
        {
            entity.HasKey(e => new { e.SongId, e.ResourceId }).HasName("PRIMARY");

            entity.HasOne(d => d.Resource).WithMany(p => p.SongResources).HasConstraintName("song_resource_resource_id_foreign");

            entity.HasOne(d => d.Song).WithMany(p => p.SongResources).HasConstraintName("song_resource_song_id_foreign");
        });

        modelBuilder.Entity<Studio>(entity =>
        {
            entity.HasKey(e => e.StudioId).HasName("PRIMARY");
        });

        modelBuilder.Entity<StudioImage>(entity =>
        {
            entity.HasKey(e => new { e.StudioId, e.ImageId }).HasName("PRIMARY");

            entity.HasOne(d => d.Image).WithMany(p => p.StudioImages).HasConstraintName("studio_image_image_id_foreign");

            entity.HasOne(d => d.Studio).WithMany(p => p.StudioImages).HasConstraintName("studio_image_studio_id_foreign");
        });

        modelBuilder.Entity<StudioResource>(entity =>
        {
            entity.HasKey(e => new { e.StudioId, e.ResourceId }).HasName("PRIMARY");

            entity.HasOne(d => d.Resource).WithMany(p => p.StudioResources).HasConstraintName("studio_resource_resource_id_foreign");

            entity.HasOne(d => d.Studio).WithMany(p => p.StudioResources).HasConstraintName("studio_resource_studio_id_foreign");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PRIMARY");

            entity.HasOne(d => d.Audio).WithMany(p => p.Videos)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("videos_audio_id_foreign");
        });

        modelBuilder.Entity<VideoScript>(entity =>
        {
            entity.HasKey(e => e.ScriptId).HasName("PRIMARY");

            entity.HasOne(d => d.Video).WithMany(p => p.VideoScripts)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("video_scripts_video_id_foreign");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
