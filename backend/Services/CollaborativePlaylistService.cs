using System.Collections.Concurrent;
using MyApi.Dtos;
using MyApi.Models;
using MyApi.Repositories;

namespace MyApi.Services
{
    public class CollaborativePlaylistService : ICollaborativePlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISongRepository _songRepository;

        // Thread-safe in-memory storage for active users
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<int, DateTime>> _activeUsers = new();
        private static readonly TimeSpan InactiveTimeout = TimeSpan.FromMinutes(1);

        public CollaborativePlaylistService(
            IPlaylistRepository playlistRepository,
            IUserRepository userRepository,
            ISongRepository songRepository)
        {
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
            _songRepository = songRepository;
        }

        // Collaborators
        public async Task<IEnumerable<UserDto>> GetCollaboratorsAsync(int playlistId)
        {
            var playlist = await _playlistRepository.GetByIdWithDetailsAsync(playlistId);
            if (playlist == null) return null!;

            return playlist.Users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                ProfileImage = u.ProfileImage
            });
        }

        // NEW: Add collaborator by username
        public async Task<(bool Success, string? Error)> AddCollaboratorByUsernameAsync(int playlistId, string username, int requesterId)
        {
            try
            {
                var playlist = await _playlistRepository.GetByIdWithDetailsAsync(playlistId);
                if (playlist == null)
                {
                    return (false, $"Playlist with ID {playlistId} not found.");
                }

                if (playlist.HostId != requesterId)
                {
                    return (false, "Only host can add collaborators.");
                }

                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    return (false, $"User with username '{username}' not found.");
                }

                if (playlist.HostId == user.Id)
                {
                    return (false, "Host is already the owner of the playlist.");
                }

                if (playlist.Users.Any(u => u.Id == user.Id))
                {
                    return (false, "User is already a collaborator.");
                }

                playlist.Users.Add(user);
                await _playlistRepository.UpdateAsync(playlist);

                return (true, null);
            }
            catch (Exception)
            {
                return (false, "An error occurred while adding collaborator.");
            }
        }

        public async Task<(bool Success, string? Error)> AddCollaboratorAsync(int playlistId, int userId, int requesterId)
        {
            try
            {
                var playlist = await _playlistRepository.GetByIdWithDetailsAsync(playlistId);
                if (playlist == null)
                {
                    return (false, $"Playlist with ID {playlistId} not found.");
                }

                if (playlist.HostId != requesterId)
                {
                    return (false, "Only host can add collaborators.");
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return (false, $"User with ID {userId} not found.");
                }

                if (playlist.Users.Any(u => u.Id == userId))
                {
                    return (false, "User is already a collaborator.");
                }

                playlist.Users.Add(user);
                await _playlistRepository.UpdateAsync(playlist);

                return (true, null);
            }
            catch (Exception)
            {
                return (false, "An error occurred while adding collaborator.");
            }
        }

        public async Task<(bool Success, string? Error)> RemoveCollaboratorAsync(int playlistId, int userId, int requesterId)
        {
            try
            {
                var playlist = await _playlistRepository.GetByIdWithDetailsAsync(playlistId);
                if (playlist == null)
                {
                    return (false, $"Playlist with ID {playlistId} not found.");
                }

                if (playlist.HostId != requesterId)
                {
                    return (false, "Only host can remove collaborators.");
                }

                var user = playlist.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return (false, "User is not a collaborator.");
                }

                playlist.Users.Remove(user);
                await _playlistRepository.UpdateAsync(playlist);

                return (true, null);
            }
            catch (Exception)
            {
                return (false, "An error occurred while removing collaborator.");
            }
        }

        // Songs
        public async Task<(bool Success, string? Error)> AddSongAsync(int playlistId, int songId, int userId)
        {
            try
            {
                var playlist = await _playlistRepository.GetByIdWithDetailsAsync(playlistId);
                if (playlist == null)
                {
                    return (false, $"Playlist with ID {playlistId} not found.");
                }

                if (playlist.HostId != userId && !playlist.Users.Any(u => u.Id == userId))
                {
                    return (false, "Only host or collaborator can add songs.");
                }

                var song = await _songRepository.GetByIdAsync(songId);
                if (song == null)
                {
                    return (false, $"Song with ID {songId} not found.");
                }

                if (playlist.PlaylistSongs.Any(ps => ps.SongId == songId))
                {
                    return (false, "Song already in playlist.");
                }

                var position = playlist.PlaylistSongs.Count + 1;

                await _playlistRepository.AddPlaylistSongAsync(new PlaylistSong
                {
                    PlaylistId = playlistId,
                    SongId = songId,
                    Position = position,
                    AddedByUserId = userId, // NEW: Track who added the song
                    AddedAt = DateTime.UtcNow // NEW: Track when it was added
                });

                return (true, null);
            }
            catch (Exception)
            {
                return (false, "An error occurred while adding song.");
            }
        }

        public async Task<(bool Success, string? Error)> RemoveSongAsync(int playlistId, int songId, int userId)
        {
            try
            {
                var playlist = await _playlistRepository.GetByIdWithDetailsAsync(playlistId);
                if (playlist == null)
                {
                    return (false, $"Playlist with ID {playlistId} not found.");
                }

                if (playlist.HostId != userId && !playlist.Users.Any(u => u.Id == userId))
                {
                    return (false, "Only host or collaborator can remove songs.");
                }

                var ps = playlist.PlaylistSongs.FirstOrDefault(p => p.SongId == songId);
                if (ps == null)
                {
                    return (false, "Song not found in playlist.");
                }

                await _playlistRepository.RemovePlaylistSongAsync(ps);

                return (true, null);
            }
            catch (Exception)
            {
                return (false, "An error occurred while removing song.");
            }
        }

        // Access
        public async Task<bool> CanAccessPlaylistAsync(int playlistId, int userId)
        {
            var playlist = await _playlistRepository.GetByIdWithDetailsAsync(playlistId);
            if (playlist == null) return false;

            return playlist.HostId == userId || playlist.Users.Any(u => u.Id == userId);
        }

        // Active Users (In-Memory)
        public Task JoinPlaylistSessionAsync(int playlistId, int userId)
        {
            var playlistUsers = _activeUsers.GetOrAdd(
                playlistId,
                _ => new ConcurrentDictionary<int, DateTime>()
            );

            playlistUsers[userId] = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        public Task LeavePlaylistSessionAsync(int playlistId, int userId)
        {
            if (_activeUsers.TryGetValue(playlistId, out var playlistUsers))
            {
                playlistUsers.TryRemove(userId, out _);

                // Cleanup empty playlist sessions
                if (playlistUsers.IsEmpty)
                {
                    _activeUsers.TryRemove(playlistId, out _);
                }
            }

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ActiveUserDto>> GetActiveUsersAsync(int playlistId)
        {
            if (!_activeUsers.TryGetValue(playlistId, out var usersDict))
                return Enumerable.Empty<ActiveUserDto>();

            // Cleanup inactive users
            var threshold = DateTime.UtcNow - InactiveTimeout;
            var inactiveUsers = usersDict
                .Where(kvp => kvp.Value < threshold)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var userId in inactiveUsers)
            {
                usersDict.TryRemove(userId, out _);
            }

            // Get all active user IDs
            var activeUserIds = usersDict.Keys.ToList();
            if (!activeUserIds.Any())
            {
                // Cleanup empty session
                _activeUsers.TryRemove(playlistId, out _);
                return Enumerable.Empty<ActiveUserDto>();
            }

            // Batch fetch users
            var users = await _userRepository.GetByIdsAsync(activeUserIds);

            var result = users.Select(user => new ActiveUserDto
            {
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role,
                LastActive = usersDict.TryGetValue(user.Id, out var lastActive) ? lastActive : DateTime.UtcNow
            }).ToList();

            return result;
        }

        // Cleanup (Optional - call from background service)
        public Task CleanupInactiveSessionsAsync()
        {
            var threshold = DateTime.UtcNow - InactiveTimeout;
            var playlistsToRemove = new List<int>();

            foreach (var playlist in _activeUsers)
            {
                var usersToRemove = playlist.Value
                    .Where(u => u.Value < threshold)
                    .Select(u => u.Key)
                    .ToList();

                foreach (var userId in usersToRemove)
                {
                    playlist.Value.TryRemove(userId, out _);
                }

                if (playlist.Value.IsEmpty)
                {
                    playlistsToRemove.Add(playlist.Key);
                }
            }

            foreach (var playlistId in playlistsToRemove)
            {
                _activeUsers.TryRemove(playlistId, out _);
            }

            return Task.CompletedTask;
        }
    }
}