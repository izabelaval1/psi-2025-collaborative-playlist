using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Dtos;
using System.Linq;
using MyApi.Utils;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly PlaylistAppContext _context;

        public PlaylistsController(PlaylistAppContext context)
        {
            _context = context;
        }

        // GET /api/playlists -> return all playlists
        [HttpGet]
        public IActionResult GetPlaylists()
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

            return Ok(playlists);
        }

        // GET /api/playlists/{id} -> return playlist by ID
        [HttpGet("{id:int}")]
        public IActionResult GetPlaylistById(int id)
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

            if (playlist == null)
                return NotFound($"Playlist with ID {id} not found.");

            return Ok(playlist);
        }

        // POST /api/playlists -> create a new playlist
        [HttpPost]
        public IActionResult CreatePlaylist([FromBody] PlaylistCreateDto newPlaylist)
        {
            var exists = _context.Playlists.Any(p => p.Name == newPlaylist.Name);
            if (exists)
                return Conflict($"A playlist with the name '{newPlaylist.Name}' already exists.");

            // Check if host exists and has proper role
            var host = _context.Users.FirstOrDefault(u => u.Id == newPlaylist.HostId);
            if (host == null)
                return NotFound($"Host with ID {newPlaylist.HostId} not found.");

            if (host.Role != UserRole.Host && host.Role != UserRole.Admin)
                return StatusCode(403, $"User with ID {newPlaylist.HostId} is not authorized to host playlists.");

            var playlist = new Playlist
            {
                Name = newPlaylist.Name,
                Description = newPlaylist.Description,
                HostId = newPlaylist.HostId
            };

            _context.Playlists.Add(playlist);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPlaylistById), new { id = playlist.Id }, playlist);
        }

        // PUT /api/playlists/by-id/{id} -> full update by ID
        [HttpPut("by-id/{id:int}")]
        public IActionResult UpdatePlaylistById(int id, [FromBody] PlaylistUpdateDto updatedPlaylist)
        {
            var existing = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .Include(p => p.Host)
                .FirstOrDefault(p => p.Id == id);

            if (existing == null)
                return NotFound();

            // Authorization check
            if (existing.Host != null && existing.Host.Role != UserRole.Host && existing.Host.Role != UserRole.Admin)
                return StatusCode(403, "Only hosts or admins can update playlists.");

            existing.Name = updatedPlaylist.Name;
            existing.Description = updatedPlaylist.Description;

            // Update PlaylistSongs
            if (updatedPlaylist.SongIds != null)
            {
                // Remove existing playlist songs
                var existingSongs = existing.PlaylistSongs.ToList();
                foreach (var ps in existingSongs)
                {
                    _context.PlaylistSongs.Remove(ps);
                }

                // Add new playlist songs
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

            // Return updated playlist
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

            return Ok(updated);
        }

        // PATCH /api/playlists/{id} -> partial update
        [HttpPatch("{id:int}")]
        public IActionResult EditPlaylist(int id, [FromBody] PlaylistPatchDto editedPlaylist)
        {
            var existing = _context.Playlists
                .Include(p => p.Host)
                .FirstOrDefault(p => p.Id == id);

            if (existing == null)
                return NotFound();

            // Authorization check
            if (existing.Host != null && existing.Host.Role != UserRole.Host && existing.Host.Role != UserRole.Admin)
                return StatusCode(403, "Only hosts or admins can edit playlists.");

            if (!string.IsNullOrEmpty(editedPlaylist.Name))
                existing.Name = editedPlaylist.Name;

            if (editedPlaylist.Description != null)
                existing.Description = editedPlaylist.Description;

            _context.SaveChanges();

            // Return updated playlist
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

            return Ok(updated);
        }

        // DELETE /api/playlists/{id} -> delete playlist by ID
        [HttpDelete("{id:int}")]
        public IActionResult DeletePlaylistById(int id)
        {
            var playlist = _context.Playlists
                .Include(p => p.Host)
                .FirstOrDefault(p => p.Id == id);

            if (playlist == null)
                return NotFound();

            // Authorization check
            if (playlist.Host != null && playlist.Host.Role != UserRole.Host && playlist.Host.Role != UserRole.Admin)
                return StatusCode(403, "Only hosts or admins can delete playlists.");

            _context.Playlists.Remove(playlist);
            _context.SaveChanges();

            return NoContent();
        }
    }
}