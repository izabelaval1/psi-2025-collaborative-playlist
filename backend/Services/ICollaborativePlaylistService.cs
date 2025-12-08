using MyApi.Dtos;
public interface ICollaborativePlaylistService
{
    Task<IEnumerable<UserDto>> GetCollaboratorsAsync(int playlistId);
    Task<(bool Success, string? Error)> AddCollaboratorAsync(int playlistId, int userId, int requesterId);

    Task<(bool Success, string? Error)> AddCollaboratorByUsernameAsync(int playlistId, string username, int requesterId);

    Task<(bool Success, string? Error)> RemoveCollaboratorAsync(int playlistId, int userId, int requesterId);

    Task<(bool Success, string? Error)> AddSongAsync(int playlistId, int songId, int userId);
    Task<(bool Success, string? Error)> RemoveSongAsync(int playlistId, int songId, int userId);

    Task<bool> CanAccessPlaylistAsync(int playlistId, int userId);

    Task JoinPlaylistSessionAsync(int playlistId, int userId);
    Task LeavePlaylistSessionAsync(int playlistId, int userId);
    Task<IEnumerable<ActiveUserDto>> GetActiveUsersAsync(int playlistId);
    
    Task CleanupInactiveSessionsAsync();
}