using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/Song")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;
        public SongsController(ISongService songService) => _songService = songService;

        // POST /api/songs/add-to-playlist
        [HttpPost("add-to-playlist")]
        public async Task<IActionResult> AddSongToPlaylist([FromBody] AddSongToPlaylistDto request)
        {
            var (success, error, songId) = await _songService.AddSongToPlaylistAsync(request);

            if (!success)
            {
                if (!string.IsNullOrEmpty(error) && error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(error);
                if (!string.IsNullOrEmpty(error) && error.Contains("already in the playlist", StringComparison.OrdinalIgnoreCase))
                    return Conflict(error);
                return BadRequest(error);
            }

            return Ok(new { message = "Song added successfully", songId });
        }

        // GET /api/songs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var songs = await _songService.GetAllAsync();
            return Ok(songs);
        }

        // GET /api/songs/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var song = await _songService.GetByIdAsync(id);
            if (song == null) return NotFound($"Song with ID {id} not found.");
            return Ok(song);
        }

        // DELETE /api/songs/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _songService.DeleteAsync(id);
            if (!success)
            {
                if (!string.IsNullOrEmpty(error) && error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(error);
                return BadRequest(error);
            }
            return Ok(new { message = "Song deleted successfully" });
        }
    }
}
