using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Dtos;
using MyApi.Utils;
using MyApi.Interfaces;

namespace MyApi.Services
{
    /// <summary>
    /// Servisas atsakingas už visą logiką, susijusią su grojarąščiais (playlists).
    /// Controlleriai tik kviečia šiuos metodus – čia vyksta DB užklausos, validacijos ir DTO mapping.
    /// </summary>
    public class PlaylistService : IPlaylistService
    {
        private readonly PlaylistAppContext _context;

    
        public PlaylistService(PlaylistAppContext context)
        {
            _context = context;
        }

        // ============================================================
        //  GET: Gauti visus grojarąščius
        // ============================================================
        public IEnumerable<PlaylistResponseDto> GetAllPlaylists()
        {
            //  Pirmiausia gauname duomenis iš DB
            var playlists = _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artists)
                .Include(p => p.Users)
                .Include(p => p.Host)
                .AsNoTracking()
                .ToList();  // Execute query and bring to memory

            var converter = new GenericConverter<Playlist, PlaylistResponseDto>();
            // Tada konvertuojame į DTO atmintyje
            var playlistDtos = converter.ConvertAll(playlists, p => new PlaylistResponseDto
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

                // Dainų sąrašas su Duration konvertavimu
                Songs = p.PlaylistSongs
                    .OrderBy(ps => ps.Position)
                    .Select(ps => new SongDto
                    {
                        Id = ps.Song.Id,
                        Title = ps.Song.Title,
                        Album = ps.Song.Album,

                        // Konvertuojame seconds -> Duration -> tekstas "MM:SS"
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
            }).ToList();

            return playlistDtos;
        }

        // ============================================================
        //  GET by ID: Gauti konkretų grojarąštį
        // ============================================================
        public PlaylistResponseDto? GetPlaylistById(int id)
        {
            //  Pirmiausia gauname entitą iš DB
            // viena pasiimam eilute is playlists lenteles su tuo id kuris perduotas ir sukuriam objekta
            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artists)
                .Include(p => p.Users)
                .Include(p => p.Host)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);

            if (playlist == null)
                return null;

            var converter = new GenericConverter<Playlist, PlaylistResponseDto>();

            //  Tada konvertuojame į DTO atmintyje
            var dto = converter.ConvertOne(playlist, playlist => new PlaylistResponseDto
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Description = playlist.Description,
                HostId = playlist.HostId,

                Host = playlist.Host != null ? new UserDto
                {
                    Id = playlist.Host.Id,
                    Username = playlist.Host.Username,
                    Role = playlist.Host.Role
                } : null,

                Songs = playlist.PlaylistSongs
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

                Collaborators = playlist.Users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role
                }).ToList()
            }
            );

            return dto;
        }

        // ============================================================
        //  POST: Sukurti naują grojarąštį
        // ============================================================
        public (bool Success, string? Error, PlaylistResponseDto? Created) CreatePlaylist(PlaylistCreateDto newPlaylist)
        {
            if (_context.Playlists.Any(p => p.Name == newPlaylist.Name))
                return (false, $"A playlist named '{newPlaylist.Name}' already exists.", null);

            var host = _context.Users.FirstOrDefault(u => u.Id == newPlaylist.HostId);
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

            _context.Playlists.Add(playlist);
            _context.SaveChanges();

            // Gauname sukurtą grojarąštį kaip DTO
            var createdDto = GetPlaylistById(playlist.Id);
            return (true, null, createdDto);
        }

        // ============================================================
        //  PUT: Pilnas grojarąščio atnaujinimas
        // ============================================================
        public (bool Success, string? Error, PlaylistResponseDto? Updated) UpdatePlaylistById(int id, PlaylistUpdateDto updatedPlaylist)
        {
            var existing = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .Include(p => p.Host)
                .FirstOrDefault(p => p.Id == id);

            if (existing == null)
                return (false, $"Playlist with ID {id} not found.", null);

            if (existing.Host != null && existing.Host.Role != UserRole.Host && existing.Host.Role != UserRole.Admin)
                return (false, "Only hosts or admins can update playlists.", null);

            existing.Name = updatedPlaylist.Name;
            existing.Description = updatedPlaylist.Description;

            if (updatedPlaylist.SongIds != null)
            {
                var existingSongs = existing.PlaylistSongs.ToList();
                foreach (var ps in existingSongs)
                    _context.PlaylistSongs.Remove(ps);

                var position = 1;
                foreach (var songId in updatedPlaylist.SongIds)
                {
                    var dbSong = _context.Songs.FirstOrDefault(s => s.Id == songId);
                    if (dbSong != null)
                    {
                        _context.PlaylistSongs.Add(new PlaylistSong
                        {
                            PlaylistId = existing.Id,
                            SongId = dbSong.Id,
                            Position = position++
                        });
                    }
                }
            }

            _context.SaveChanges();

            //  Gauname atnaujintą grojarąštį kaip DTO
            var updated = GetPlaylistById(id);
            return (true, null, updated);
        }

        // ============================================================
        // PATCH: Dalinis grojarąščio redagavimas
        // ============================================================
        public (bool Success, string? Error, PlaylistResponseDto? Updated) EditPlaylist(int id, PlaylistPatchDto editedPlaylist)
        {
            // uzkrauname playlista ir useri (hosta) is db
            var existing = _context.Playlists
                .Include(p => p.Host)
                .FirstOrDefault(p => p.Id == id);

            if (existing == null)
                return (false, $"Playlist with ID {id} not found.", null);

            if (existing.Host != null && existing.Host.Role != UserRole.Host && existing.Host.Role != UserRole.Admin)
                return (false, "Only hosts or admins can edit playlists.", null);

            if (!string.IsNullOrEmpty(editedPlaylist.Name))
                existing.Name = editedPlaylist.Name;

            if (editedPlaylist.Description != null)
                existing.Description = editedPlaylist.Description;

            _context.SaveChanges();

            // Gauname atnaujintą grojarąštį kaip DTO
            var updated = GetPlaylistById(id);
            return (true, null, updated);
        }

        // ============================================================
        //  DELETE: Pašalinti grojarąštį
        // ============================================================
        public (bool Success, string? Error) DeletePlaylist(int id)
        {
            var playlist = _context.Playlists
                .Include(p => p.Host)
                .Include(p => p.PlaylistSongs)
                .FirstOrDefault(p => p.Id == id);
              

            if (playlist == null)
                return (false, $"Playlist with ID {id} not found.");

            if (playlist.Host != null && playlist.Host.Role != UserRole.Host && playlist.Host.Role != UserRole.Admin)
                return (false, "Only hosts or admins can delete playlists.");

            if (playlist.PlaylistSongs.Any())
                _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);

            _context.Playlists.Remove(playlist);
            _context.SaveChanges();

            return (true, null);
        }
        // ============================================================
        //  DELETE: Pašalinti dainą iš grojarąščio
        // ============================================================
        public (bool Success, string? Error) RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var playlistSong = _context.PlaylistSongs
                .FirstOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistSong == null)
                return (false, "This song is not found in the playlist.");

            _context.PlaylistSongs.Remove(playlistSong);
            _context.SaveChanges();

            return (true, null);
        }
    }
}