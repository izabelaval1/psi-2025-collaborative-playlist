using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Dtos;
using MyApi.Utils;

namespace MyApi.Services
{
    public class PlaylistService
    {
        private readonly PlaylistAppContext _context;

        public PlaylistService(PlaylistAppContext context)
        {
            _context = context;
        }

        // Čia kelsim metodus iš controller
        public IEnumerable<PlaylistResponseDto> GetAllPlaylists()
        {
            var playlists = _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artists)
                .Include(p => p.Users)
                .Include(p => p.Host)
                .AsNoTracking()
                .AsEnumerable()
                .Select(p => new PlaylistResponseDto
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
                        .OrderedByPosition()
                        .Select(ps => new SongDto
                        {
                            Id = ps.Song.Id,
                            Title = ps.Song.Title,
                            Album = ps.Song.Album,
                            Duration = ps.Song.Duration,
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
                })
                .ToList();

            return playlists;
        }

        public PlaylistResponseDto? GetPlaylistById(int id)
        {
            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artists)
                .Include(p => p.Users)
                .Include(p => p.Host)
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PlaylistResponseDto
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
                            Duration = ps.Song.Duration,
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
                })
                .FirstOrDefault();

            return playlist;
        }

        public (bool Success, string? Error, Playlist? Created) CreatePlaylist(PlaylistCreateDto newPlaylist)
        {
            // 1️⃣ Check if playlist with same name exists
            var exists = _context.Playlists.Any(p => p.Name == newPlaylist.Name);
            if (exists)
                return (false, $"A playlist with the name '{newPlaylist.Name}' already exists.", null);

            // 2️⃣ Check if host exists
            var host = _context.Users.FirstOrDefault(u => u.Id == newPlaylist.HostId);
            if (host == null)
                return (false, $"Host with ID {newPlaylist.HostId} not found.", null);

            // 3️⃣ Role validation
            if (host.Role != UserRole.Host && host.Role != UserRole.Admin)
                return (false, $"User with ID {newPlaylist.HostId} is not authorized to host playlists.", null);

            // 4️⃣ Create playlist
            var playlist = new Playlist
            {
                Name = newPlaylist.Name,
                Description = newPlaylist.Description,
                HostId = newPlaylist.HostId
            };

            _context.Playlists.Add(playlist);
            _context.SaveChanges();

            // 5️⃣ Return success + created object
            return (true, null, playlist);
        }

        public (bool Success, string? Error, PlaylistResponseDto? Updated) UpdatePlaylistById(int id, PlaylistUpdateDto updatedPlaylist)
        {
            // 1️⃣ Find existing playlist
            var existing = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .Include(p => p.Host)
                .FirstOrDefault(p => p.Id == id);

            if (existing == null)
                return (false, $"Playlist with ID {id} not found.", null);

            // 2️⃣ Authorization check
            if (existing.Host != null && existing.Host.Role != UserRole.Host && existing.Host.Role != UserRole.Admin)
                return (false, "Only hosts or admins can update playlists.", null);

            // 3️⃣ Basic updates
            existing.Name = updatedPlaylist.Name;
            existing.Description = updatedPlaylist.Description;

            // 4️⃣ If song list provided – replace songs
            if (updatedPlaylist.SongIds != null)
            {
                // Remove existing playlist songs
                var existingSongs = existing.PlaylistSongs.ToList();
                foreach (var ps in existingSongs)
                    _context.PlaylistSongs.Remove(ps);

                // Add new ones
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

            // 5️⃣ Reload updated playlist and map to DTO
            var updated = _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artists)
                .Include(p => p.Users)
                .Include(p => p.Host)
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PlaylistResponseDto
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
                            Duration = ps.Song.Duration,
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
                })
                .FirstOrDefault();

            return (true, null, updated);
        }


        public (bool Success, string? Error, PlaylistResponseDto? Updated) EditPlaylist(int id, PlaylistPatchDto editedPlaylist)
        {
            // 1️⃣ Rasti esamą playlist
            var existing = _context.Playlists
                .Include(p => p.Host)
                .FirstOrDefault(p => p.Id == id);

            if (existing == null)
                return (false, $"Playlist with ID {id} not found.", null);

            // 2️⃣ Leidimų tikrinimas
            if (existing.Host != null && existing.Host.Role != UserRole.Host && existing.Host.Role != UserRole.Admin)
                return (false, "Only hosts or admins can edit playlists.", null);

            // 3️⃣ Atnaujinti tik nurodytus laukus
            if (!string.IsNullOrEmpty(editedPlaylist.Name))
                existing.Name = editedPlaylist.Name;

            if (editedPlaylist.Description != null)
                existing.Description = editedPlaylist.Description;

            _context.SaveChanges();

            // 4️⃣ Užkrauti naujai atnaujintą playlist kaip DTO
            var updated = _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artists)
                .Include(p => p.Users)
                .Include(p => p.Host)
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PlaylistResponseDto
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
                            Duration = ps.Song.Duration,
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
                })
                .FirstOrDefault();

            return (true, null, updated);
        }
            
            public (bool Success, string? Error) DeletePlaylist(int id)
{
    // 1️⃣ Rasti playlist
    var playlist = _context.Playlists
        .Include(p => p.Host)
        .FirstOrDefault(p => p.Id == id);

    if (playlist == null)
        return (false, $"Playlist with ID {id} not found.");

    // 2️⃣ Autorizacijos tikrinimas
    if (playlist.Host != null && playlist.Host.Role != UserRole.Host && playlist.Host.Role != UserRole.Admin)
        return (false, "Only hosts or admins can delete playlists.");

    // 3️⃣ Ištrinti playlist
    _context.Playlists.Remove(playlist);
    _context.SaveChanges();

    return (true, null);
}




    }
}
