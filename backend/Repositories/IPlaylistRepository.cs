using MyApi.Models;

namespace MyApi.Repositories
{
    /// <summary>
    /// Repository abstraction for playlist data access.
    /// Service layer should use this instead of DbContext directly.
    /// </summary>
    public interface IPlaylistRepository
    {
        Task<IEnumerable<Playlist>> GetAllAsync();
        Task<Playlist?> GetByIdAsync(int id);
        Task<Playlist?> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Playlist playlist);
        Task UpdateAsync(Playlist playlist);
        Task DeleteAsync(Playlist playlist);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByNameAsync(string name);
        
        // PlaylistSong operations
        Task<PlaylistSong?> GetPlaylistSongAsync(int playlistId, int songId);
        Task RemovePlaylistSongAsync(PlaylistSong playlistSong);
        Task AddPlaylistSongAsync(PlaylistSong playlistSong);
        Task RemovePlaylistSongsRangeAsync(IEnumerable<PlaylistSong> playlistSongs);
        
        // Helper methods for validation
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> SongExistsAsync(int songId);
        
        Task SaveChangesAsync();
    }
}