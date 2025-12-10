using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly PlaylistAppContext _db;

        public PlaylistRepository(PlaylistAppContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Playlist>> GetAllAsync()
        {
            return await _db.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artists)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.AddedBy)
                .Include(p => p.Users)
                .Include(p => p.Host)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Playlist?> GetByIdAsync(int id)
        {
            return await _db.Playlists
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Playlist?> GetByIdWithDetailsAsync(int id)
        {
            return await _db.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artists)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.AddedBy)
                .Include(p => p.Users)
                .Include(p => p.Host)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Playlist playlist)
        {
            _db.Playlists.Add(playlist);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Playlist playlist)
        {
            _db.Playlists.Update(playlist);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Playlist playlist)
        {
            _db.Playlists.Remove(playlist);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Playlists.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Playlists.AnyAsync(p => p.Name == name);
        }

        public async Task<PlaylistSong?> GetPlaylistSongAsync(int playlistId, int songId)
        {
            return await _db.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
        }

        public async Task RemovePlaylistSongAsync(PlaylistSong playlistSong)
        {
            _db.PlaylistSongs.Remove(playlistSong);
            await _db.SaveChangesAsync();
        }

        public async Task AddPlaylistSongAsync(PlaylistSong playlistSong)
        {
            _db.PlaylistSongs.Add(playlistSong);
            await _db.SaveChangesAsync();
        }

        public async Task RemovePlaylistSongsRangeAsync(IEnumerable<PlaylistSong> playlistSongs)
        {
            _db.PlaylistSongs.RemoveRange(playlistSongs);
            await _db.SaveChangesAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> SongExistsAsync(int songId)
        {
            return await _db.Songs.AnyAsync(s => s.Id == songId);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}