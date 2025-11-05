using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyApi.Models;

public partial class PlaylistAppContext : DbContext
{
    //public PlaylistAppContext()
    //{
    //}

        public PlaylistAppContext(DbContextOptions<PlaylistAppContext> options)
            : base(options)
        {
        }

    public virtual DbSet<Artist> Artists { get; set; }
    public virtual DbSet<Playlist> Playlists { get; set; }
    public virtual DbSet<PlaylistSong> PlaylistSongs { get; set; }
    public virtual DbSet<Song> Songs { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("artists_pkey");
            entity.ToTable("artists");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PasswordHash).HasMaxLength(255).HasColumnName("password_hash");
            entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username");

            entity.Property(e => e.Role)
            .HasColumnName("role")
            .HasMaxLength(20)
            .HasConversion<int>(); 
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("songs_pkey");
            entity.ToTable("songs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Album).HasMaxLength(100).HasColumnName("album");
           // entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Title).HasMaxLength(100).HasColumnName("title");

            // Song-Artist many-to-many
            entity.HasMany(d => d.Artists)
                .WithMany(p => p.Songs)
                .UsingEntity<Dictionary<string, object>>(
                    "song_artists",
                    r => r.HasOne<Artist>().WithMany().HasForeignKey("artist_id"),
                    l => l.HasOne<Song>().WithMany().HasForeignKey("song_id"),
                    j =>
                    {
                        j.HasKey("song_id", "artist_id");
                        j.ToTable("song_artists");
                    });
        });

        modelBuilder.Entity<PlaylistSong>(entity =>
        {
            entity.ToTable("playlist_songs");
            entity.HasKey(e => new { e.PlaylistId, e.SongId }).HasName("playlist_songs_pkey");

            entity.Property(e => e.PlaylistId).HasColumnName("playlist_id");
            entity.Property(e => e.SongId).HasColumnName("song_id");
            entity.Property(e => e.Position).HasColumnName("position");

            entity.HasOne(d => d.Playlist)
                  .WithMany(p => p.PlaylistSongs)
                  .HasForeignKey(d => d.PlaylistId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Song)
                  .WithMany(p => p.PlaylistSongs)
                  .HasForeignKey(d => d.SongId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.ToTable("playlists");
            entity.HasKey(p => p.Id).HasName("playlists_pkey");

            entity.Property(p => p.Id).HasColumnName("id");
            entity.Property(p => p.Name).HasMaxLength(100).HasColumnName("name");
            entity.Property(p => p.Description).HasColumnName("description");
            entity.Property(p => p.HostId).HasColumnName("host_id");

            entity.HasOne(p => p.Host)
                  .WithMany(u => u.HostedPlaylists)
                  .HasForeignKey(p => p.HostId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(p => p.Users)
                  .WithMany(u => u.CollaboratingPlaylists)
                  .UsingEntity<Dictionary<string, object>>(
                      "playlist_collaborators",
                      r => r.HasOne<User>().WithMany().HasForeignKey("user_id"),
                      l => l.HasOne<Playlist>().WithMany().HasForeignKey("playlist_id"),
                      j =>
                      {
                          j.HasKey("playlist_id", "user_id");
                          j.ToTable("playlist_collaborators");
                      });
        });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
