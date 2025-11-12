using MyApi.Dtos;

namespace MyApi.Services
{
    public interface IPlaylistService
    {
        Task<IEnumerable<PlaylistResponseDto>> GetAllAsync();
        Task<PlaylistResponseDto?> GetByIdAsync(int id);
        Task<(bool Success, string? Error, PlaylistResponseDto? Created)> CreateAsync(PlaylistCreateDto newPlaylist);
        Task<(bool Success, string? Error, PlaylistResponseDto? Updated)> UpdateByIdAsync(int id, PlaylistUpdateDto updatedPlaylist);
        Task<(bool Success, string? Error, PlaylistResponseDto? Updated)> EditAsync(int id, PlaylistPatchDto editedPlaylist);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
        Task<(bool Success, string? Error)> RemoveSongFromPlaylistAsync(int playlistId, int songId);
    }
}