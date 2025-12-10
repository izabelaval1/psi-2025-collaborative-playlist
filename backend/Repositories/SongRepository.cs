using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly PlaylistAppContext _db;

        public SongRepository(PlaylistAppContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Song>> GetAllAsync()
        {
            return await _db.Songs
                .Include(s => s.Artists)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Song?> GetByIdAsync(int id)
        {
            return await _db.Songs
                .Include(s => s.Artists)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Songs.AnyAsync(s => s.Id == id);
        }

        public async Task AddAsync(Song song)
        {
            _db.Songs.Add(song);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Song song)
        {
            _db.Songs.Update(song);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Song song)
        {
            _db.Songs.Remove(song);
            await _db.SaveChangesAsync();
        }

        public async Task<Song?> FindByTitleAndAlbumAsync(string title, string? album)
        {
            return await _db.Songs
                .Include(s => s.Artists)
                .FirstOrDefaultAsync(s => s.Title == title && s.Album == album);
        }

        public async Task<Song> EnsureSongWithArtistsAsync(
            string title,
            string? album,
            int? durationSeconds,
            IEnumerable<string> artistNames,
            string spotifyId,
            string spotifyUri)
        {
            var existing = await _db.Songs
                .Include(s => s.Artists)
                .FirstOrDefaultAsync(s => s.SpotifyId == spotifyId);

            if (existing != null)
            {
                // 🔑 Ensure fields are updated
                existing.Title = title;
                existing.Album = album;
                existing.DurationSeconds = durationSeconds;

                if (string.IsNullOrEmpty(existing.SpotifyUri) || existing.SpotifyUri != spotifyUri)
                    existing.SpotifyUri = spotifyUri;

                // Update artists if needed
                var names = artistNames
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var name in names)
                {
                    if (!existing.Artists.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var artist = await _db.Artists.FirstOrDefaultAsync(a => a.Name == name)
                                     ?? new Artist { Name = name };
                        existing.Artists.Add(artist);
                    }
                }

                await _db.SaveChangesAsync();
                return existing;
            }

            // Otherwise create new
            var song = new Song
            {
                Title = title,
                Album = album,
                DurationSeconds = durationSeconds,
                SpotifyId = spotifyId,
                SpotifyUri = spotifyUri
            };

            var newNames = artistNames
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var name in newNames)
            {
                var artist = await _db.Artists.FirstOrDefaultAsync(a => a.Name == name)
                             ?? new Artist { Name = name };
                song.Artists.Add(artist);
            }

            _db.Songs.Add(song);
            await _db.SaveChangesAsync();

            return song;
        }

    }
}