using MyApi.Dtos;

namespace MyApi.Services
{
    public interface ISongService
    {
        Task<IEnumerable<SongDto>> GetAllAsync();
        Task<SongDto?> GetByIdAsync(int id);
        Task<(bool Success, string? Error)> DeleteAsync(int id);

        /// <summary>
        /// Užtikrina dainą pagal pateiktą informaciją ir prideda ją į nurodytą grojaraštį.
        /// Grąžina sukurtos/naudojamos dainos ID.
        /// </summary>
        Task<(bool Success, string? Error, int? SongId)> AddSongToPlaylistAsync(AddSongToPlaylistDto request);
    }
}
