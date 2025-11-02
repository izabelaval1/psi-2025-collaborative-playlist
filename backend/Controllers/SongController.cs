using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly SongService _songService;

        public SongsController(SongService songService)
        {
            _songService = songService;
        }

        // POST /api/songs/add-to-playlist -> pridėti dainą į playlist
        [HttpPost("add-to-playlist")]
        public IActionResult AddSongToPlaylist([FromBody] AddSongToPlaylistDto request)
        {
            var (success, error, songId) = _songService.AddSongToPlaylist(request);

            if (!success)
            {
                if (error != null && error.Contains("not found"))
                    return NotFound(error);
                if (error != null && error.Contains("already in the playlist"))
                    return Conflict(error);
                return BadRequest(error);
            }

            return Ok(new { message = "Song added successfully", songId });
        }

        // GET /api/songs -> return all songs
        [HttpGet]
        public IActionResult GetAllSongs()
        {
            var songs = _songService.GetAllSongs();
            return Ok(songs);
        }

        // GET /api/songs/{id} -> grąžinti dainą pagal ID
        [HttpGet("{id:int}")]
        public IActionResult GetSongById(int id)
        {
            var song = _songService.GetSongById(id);

            if (song == null)
                return NotFound($"Song with ID {id} not found.");

            return Ok(song);
        }
        
        [HttpDelete("{id:int}")]
        public IActionResult DeleteSong(int id)
        {
            var song = _context.Songs.Find(id);
            if (song == null)
            {
                return NotFound($"Song with ID {id} not found.");
            }

            _context.Songs.Remove(song);
            _context.SaveChanges();

            return Ok(new { message = "Song deleted successfully" });
        }

    }
}