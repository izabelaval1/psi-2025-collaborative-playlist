using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Dtos;
using System.Linq;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly PlaylistAppContext _context;

        public SongsController(PlaylistAppContext context)
        {
            _context = context;
        }

        // POST /api/songs/add-to-playlist -> add a song to a playlist
        [HttpPost("add-to-playlist")]
        public IActionResult AddSongToPlaylist([FromBody] AddSongToPlaylistDto request)
        {
            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefault(p => p.Id == request.PlaylistId);

            if (playlist == null)
                return NotFound($"Playlist with ID {request.PlaylistId} not found.");

            // Parse artists (comma-separated)
            var artistNames = request.Artist.Split(',')
                .Select(a => a.Trim())
                .Where(a => !string.IsNullOrEmpty(a))
                .ToList();

            // Check if song already exists (by title and album)
            var existingSong = _context.Songs
                .Include(s => s.Artists)
                .FirstOrDefault(s => s.Title == request.Title && s.Album == request.Album);

            Song song;
            if (existingSong != null)
            {
                song = existingSong;
            }
            else
            {
                // Create new song
                song = new Song
                {
                    Title = request.Title,
                    Album = request.Album,
                    Duration = null,
                    Artists = new List<Artist>()
                };

                // Add or find artists
                foreach (var artistName in artistNames)
                {
                    var artist = _context.Artists.FirstOrDefault(a => a.Name == artistName);
                    if (artist == null)
                    {
                        artist = new Artist { Name = artistName };
                        _context.Artists.Add(artist);
                    }
                    song.Artists.Add(artist);
                }

                _context.Songs.Add(song);
                _context.SaveChanges();
            }

            // Check if song is already in the playlist
            var alreadyInPlaylist = playlist.PlaylistSongs.Any(ps => ps.SongId == song.Id);
            if (alreadyInPlaylist)
                return Conflict("This song is already in the playlist.");

            // Add song to playlist
            var maxPosition = playlist.PlaylistSongs.Any() 
                ? playlist.PlaylistSongs.Max(ps => ps.Position) 
                : 0;

            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlist.Id,
                SongId = song.Id,
                Position = maxPosition + 1
            };

            _context.PlaylistSongs.Add(playlistSong);
            _context.SaveChanges();

            return Ok(new { message = "Song added successfully", songId = song.Id });
        }

        // GET /api/songs -> return all songs
        [HttpGet]
        public IActionResult GetAllSongs()
        {
            var songs = _context.Songs
                .Include(s => s.Artists)
                .AsNoTracking()
                .Select(s => new SongDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Album = s.Album,
                    Duration = s.Duration,
                    Artists = s.Artists.Select(a => new ArtistDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToList()
                })
                .ToList();

            return Ok(songs);
        }

        // GET /api/songs/{id} -> return song by ID
        [HttpGet("{id:int}")]
        public IActionResult GetSongById(int id)
        {
            var song = _context.Songs
                .Include(s => s.Artists)
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SongDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Album = s.Album,
                    Duration = s.Duration,
                    Artists = s.Artists.Select(a => new ArtistDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToList()
                })
                .FirstOrDefault();

            if (song == null)
                return NotFound($"Song with ID {id} not found.");

            return Ok(song);
        }
    }
}