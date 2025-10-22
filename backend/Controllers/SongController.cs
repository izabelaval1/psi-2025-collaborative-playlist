using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Dtos;
using MyApi.Utils;

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


        //This is a helper makes sure a Song (by Title + Album) exists in the database.
        //If it already exists return it (no duplicates)
        // If it doesn’t thennn create it, and also ensure each Artist exists (find or create) and link them to the song, then save.
        private Song EnsureSong(
            string title,
            string? album = null,        // OPTIONAL (default null)
            int? duration = null,        // OPTIONAL (default null)
            params string[] artistNames)  // OPTIONAL 
        {
            var existing = _context.Songs
            .Include(s => s.Artists) // also load the song’s related Artists in the same DB query.
            .FirstOrDefault(s => s.Title == title && s.Album == album); //returns the first matching song (by Title + Album) or null if not found.

            if (existing != null) return existing; //If a matching song exists, reuse it. This avoids duplicate rows.

            var song = new Song { Title = title, Album = album, Duration = duration };

            foreach (var artistName in artistNames)
            {
                var artist = _context.Artists.FirstOrDefault(a => a.Name == artistName);

                if (artist == null)
                {
                    artist = new Artist { Name = artistName };
                }
                song.Artists.Add(artist);
            }

            _context.Songs.Add(song);
            _context.SaveChanges();

            return song;
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
            var song = EnsureSong(
                title: request.Title,
                album: request.Album,
                duration: null,
                artistNames: artistNames.ToArray());

            // Check if song is already in the playlist
            var alreadyInPlaylist = playlist.PlaylistSongs.Any(ps => ps.SongId == song.Id);
            if (alreadyInPlaylist)
                return Conflict("This song is already in the playlist.");

            // 
            //ExtensionAttribute usage
            var nextPosition = playlist.PlaylistSongs.NextPosition();

            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlist.Id,
                SongId = song.Id,
                Position = nextPosition
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