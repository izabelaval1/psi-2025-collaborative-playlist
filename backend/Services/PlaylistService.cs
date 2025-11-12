using MyApi.Models;
using MyApi.Dtos;
using MyApi.Utils;
using MyApi.Repositories;

namespace MyApi.Services
{
    /// <summary>
    /// Servisas atsakingas už visą logiką, susijusią su grojaraščiais (playlists).
    /// Controlleriai tik kviečia šiuos metodus – čia vyksta validacijos ir DTO mapping.
    /// </summary>
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISongRepository _songRepository;
        private readonly GenericConverter<Playlist, PlaylistResponseDto> _converter;

        public PlaylistService(
            IPlaylistRepository playlistRepository,
            IUserRepository userRepository,
            ISongRepository songRepository)
        {
            _playlistRepository = playlistRepository;
            _userRepository = userRepository;
            _songRepository = songRepository;
            _converter = new GenericConverter<Playlist, PlaylistResponseDto>();
        }

        // ============================================================
        //  GET: Gauti visus grojaraščius
        // ============================================================
        public async Task<IEnumerable<PlaylistResponseDto>> GetAllAsync()
        {
            var playlists = await _playlistRepository.GetAllAsync();
            var playlistsList = playlists.ToList();

            return _converter.ConvertAll(playlistsList, p => new PlaylistResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                HostId = p.HostId,
                Host = p.Host != null ? new UserDto
                {
                    Id = p.Host.Id,
                    Username = p.Host.Username,
                    Role = p.Host.Role
                } : null,
                Songs = p.PlaylistSongs
                    .OrderBy(ps => ps.Position)
                    .Select(ps => new SongDto
                    {
                        Id = ps.Song.Id,
                        Title = ps.Song.Title,
                        Album = ps.Song.Album,
                        DurationFormatted = ps.Song.DurationSeconds.HasValue
                            ? new Duration(ps.Song.DurationSeconds.Value).ToString()
                            : null,
                        Position = ps.Position,
                        Artists = ps.Song.Artists.Select(a => new ArtistDto
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList()
                    }).ToList(),
                Collaborators = p.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role
                }).ToList()
            });
        }

        // ============================================================
        //  GET by ID: Gauti konkretų grojaraštį
        // ============================================================
        public async Task<PlaylistResponseDto?> GetByIdAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdWithDetailsAsync(id);
            if (playlist == null) return null;

            return _converter.ConvertOne(playlist, p => new PlaylistResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                HostId = p.HostId,
                Host = p.Host != null ? new UserDto
                {
                    Id = p.Host.Id,
                    Username = p.Host.Username,
                    Role = p.Host.Role
                } : null,
                Songs = p.PlaylistSongs
                    .OrderBy(ps => ps.Position)
                    .Select(ps => new SongDto
                    {
                        Id = ps.Song.Id,
                        Title = ps.Song.Title,
                        Album = ps.Song.Album,
                        DurationFormatted = ps.Song.DurationSeconds.HasValue
                            ? new Duration(ps.Song.DurationSeconds.Value).ToString()
                            : null,
                        Position = ps.Position,
                        Artists = ps.Song.Artists.Select(a => new ArtistDto
                        {
                            Id = a.Id,
                            Name = a.Name
                        }).ToList()
                    }).ToList(),
                Collaborators = p.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role
                }).ToList()
            });
        }

        // ============================================================
        //  POST: Sukurti naują grojaraštį
        // ============================================================
        public async Task<(bool Success, string? Error, PlaylistResponseDto? Created)> CreateAsync(PlaylistCreateDto newPlaylist)
        {
            if (await _playlistRepository.ExistsByNameAsync(newPlaylist.Name))
                return (false, $"A playlist named '{newPlaylist.Name}' already exists.", null);

            var host = await _userRepository.GetByIdAsync(newPlaylist.HostId);
            if (host == null)
                return (false, $"Host with ID {newPlaylist.HostId} not found.", null);

            if (host.Role != UserRole.Host && host.Role != UserRole.Admin)
                return (false, "User not authorized to create playlists.", null);

            var playlist = new Playlist
            {
                Name = newPlaylist.Name,
                Description = newPlaylist.Description,
                HostId = newPlaylist.HostId
            };

            await _playlistRepository.AddAsync(playlist);

            var createdDto = await GetByIdAsync(playlist.Id);
            return (true, null, createdDto);
        }

        // ============================================================
        //  PUT: Pilnas grojaraščio atnaujinimas
        // ============================================================
        public async Task<(bool Success, string? Error, PlaylistResponseDto? Updated)> UpdateByIdAsync(int id, PlaylistUpdateDto updatedPlaylist)
        {
            var existing = await _playlistRepository.GetByIdWithDetailsAsync(id);
            if (existing == null)
                return (false, $"Playlist with ID {id} not found.", null);

            if (existing.Host != null && existing.Host.Role != UserRole.Host && existing.Host.Role != UserRole.Admin)
                return (false, "Only hosts or admins can update playlists.", null);

            existing.Name = updatedPlaylist.Name;
            existing.Description = updatedPlaylist.Description;

            if (updatedPlaylist.SongIds != null)
            {
                var existingSongs = existing.PlaylistSongs.ToList();
                await _playlistRepository.RemovePlaylistSongsRangeAsync(existingSongs);

                var position = 1;
                foreach (var songId in updatedPlaylist.SongIds)
                {
                    var dbSong = await _songRepository.GetByIdAsync(songId);
                    if (dbSong != null)
                    {
                        await _playlistRepository.AddPlaylistSongAsync(new PlaylistSong
                        {
                            PlaylistId = existing.Id,
                            SongId = dbSong.Id,
                            Position = position++
                        });
                    }
                }
            }

            await _playlistRepository.UpdateAsync(existing);

            var updated = await GetByIdAsync(id);
            return (true, null, updated);
        }

        // ============================================================
        // PATCH: Dalinis grojaraščio redagavimas
        // ============================================================
        public async Task<(bool Success, string? Error, PlaylistResponseDto? Updated)> EditAsync(int id, PlaylistPatchDto editedPlaylist)
        {
            var existing = await _playlistRepository.GetByIdWithDetailsAsync(id);
            if (existing == null)
                return (false, $"Playlist with ID {id} not found.", null);

            if (existing.Host != null && existing.Host.Role != UserRole.Host && existing.Host.Role != UserRole.Admin)
                return (false, "Only hosts or admins can edit playlists.", null);

            if (!string.IsNullOrEmpty(editedPlaylist.Name))
                existing.Name = editedPlaylist.Name;

            if (editedPlaylist.Description != null)
                existing.Description = editedPlaylist.Description;

            await _playlistRepository.UpdateAsync(existing);

            var updated = await GetByIdAsync(id);
            return (true, null, updated);
        }

        // ============================================================
        //  DELETE: Pašalinti grojaraštį
        // ============================================================
        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdWithDetailsAsync(id);
            if (playlist == null)
                return (false, $"Playlist with ID {id} not found.");

            if (playlist.Host != null && playlist.Host.Role != UserRole.Host && playlist.Host.Role != UserRole.Admin)
                return (false, "Only hosts or admins can delete playlists.");

            if (playlist.PlaylistSongs.Any())
                await _playlistRepository.RemovePlaylistSongsRangeAsync(playlist.PlaylistSongs);

            await _playlistRepository.DeleteAsync(playlist);

            return (true, null);
        }

        // ============================================================
        //  DELETE: Pašalinti dainą iš grojaraščio
        // ============================================================
        public async Task<(bool Success, string? Error)> RemoveSongFromPlaylistAsync(int playlistId, int songId)
        {
            var playlistSong = await _playlistRepository.GetPlaylistSongAsync(playlistId, songId);
            if (playlistSong == null)
                return (false, "This song is not found in the playlist.");

            await _playlistRepository.RemovePlaylistSongAsync(playlistSong);

            return (true, null);
        }
    }
}