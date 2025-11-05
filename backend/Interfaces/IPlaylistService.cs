using MyApi.Dtos;

namespace MyApi.Interfaces
{
    public interface IPlaylistService
    {
        IEnumerable<PlaylistResponseDto> GetAllPlaylists();
        PlaylistResponseDto? GetPlaylistById(int id);
        (bool Success, string? Error, PlaylistResponseDto? Created) CreatePlaylist(PlaylistCreateDto newPlaylist);
        (bool Success, string? Error, PlaylistResponseDto? Updated) UpdatePlaylistById(int id, PlaylistUpdateDto updatedPlaylist);
        (bool Success, string? Error, PlaylistResponseDto? Updated) EditPlaylist(int id, PlaylistPatchDto editedPlaylist);
        (bool Success, string? Error) DeletePlaylist(int id);
        (bool Success, string? Error) RemoveSongFromPlaylist(int playlistId, int songId);
    }
}
