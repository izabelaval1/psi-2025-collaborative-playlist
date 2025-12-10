using MyApi.Models;

namespace MyApi.Repositories
{
    public interface ISongRepository
    {
        Task<IEnumerable<Song>> GetAllAsync();
        Task<Song?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddAsync(Song song);
        Task UpdateAsync(Song song);
        Task DeleteAsync(Song song);

        Task<Song?> FindByTitleAndAlbumAsync(string title, string? album);

        /// <summary>
        /// Grąžina esamą dainą pagal (title, album) arba sukuria naują su nurodytais atlikėjais.
        /// Užtikrina Artist įrašus bei sujungimus.
        /// </summary>
        Task<Song> EnsureSongWithArtistsAsync(
            string title,
            string? album,
            int? durationSeconds,
            IEnumerable<string> artistNames,
            string spotifyId,
            string spotifyUri);
    }
}
