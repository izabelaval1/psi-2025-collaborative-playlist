using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly PlaylistService _playlistService;

        public PlaylistsController(PlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        // GET /api/playlists -> grąžinti visas playlists
        [HttpGet]
        public IActionResult GetPlaylists()
        {
            var playlists = _playlistService.GetAllPlaylists();
            return Ok(playlists);
        }

        // GET /api/playlists/{id} -> grąžinti playlist pagal ID
        [HttpGet("{id:int}")]
        public IActionResult GetPlaylistById(int id)
        {
            var playlist = _playlistService.GetPlaylistById(id);

            if (playlist == null)
                return NotFound($"Playlist with ID {id} not found.");

            return Ok(playlist);
        }

        // POST /api/playlists -> sukurti naują playlist
        [HttpPost]
        public IActionResult CreatePlaylist([FromBody] PlaylistCreateDto newPlaylist)
        {
            var (success, error, created) = _playlistService.CreatePlaylist(newPlaylist);

            if (!success)
                return BadRequest(error);

            return CreatedAtAction(nameof(GetPlaylistById), new { id = created!.Id }, created);
        }

        // PUT /api/playlists/by-id/{id} -> pilnas atnaujinimas pagal ID
        [HttpPut("by-id/{id:int}")]
        // [FromBody] - nurodo, kad duomenys bus paimti iš užklausos kūno ir paildyti į PlaylistUpdateDto objektą
        public IActionResult UpdatePlaylistById(int id, [FromBody] PlaylistUpdateDto updatedPlaylist)
        {
            var (success, error, updated) = _playlistService.UpdatePlaylistById(id, updatedPlaylist);

            if (!success)
            {
                if (error != null && error.Contains("not found"))
                    return NotFound(error);
                if (error != null && error.Contains("Only hosts"))
                    return StatusCode(403, error);
                return BadRequest(error);
            }

            return Ok(updated);
        }

        // PATCH /api/playlists/{id} -> dalinis atnaujinimas
        [HttpPatch("{id:int}")]
        // grazina http atsakyma
        public IActionResult EditPlaylist(int id, [FromBody] PlaylistPatchDto editedPlaylist)
        {
            var (success, error, updated) = _playlistService.EditPlaylist(id, editedPlaylist);

            if (!success)
            {
                if (error != null && error.Contains("not found"))
                    return NotFound(error);
                if (error != null && error.Contains("Only hosts"))
                    return StatusCode(403, error);
                return BadRequest(error);
            }

            return Ok(updated);
        }

        // DELETE /api/playlists/{id} -> ištrinti playlist pagal ID
        [HttpDelete("{id:int}")]
        public IActionResult DeletePlaylistById(int id)
        {
            var (success, error) = _playlistService.DeletePlaylist(id);

            if (!success)
            {
                if (error != null && error.Contains("not found"))
                    return NotFound(error);
                if (error != null && error.Contains("Only hosts"))
                    return StatusCode(403, error);
                return BadRequest(error);
            }

            return NoContent();
        }

        [HttpDelete("{playlistId:int}/song/{songId:int}")]
        public IActionResult RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var (success, error) = _playlistService.RemoveSongFromPlaylist(playlistId, songId);

            if (!success)
            {
                if (error != null && error.Contains("not found"))
                    return NotFound(error);
                return BadRequest(error);
            }

            return Ok(new { message = "Song removed from playlist successfully" });
        }

    }
}