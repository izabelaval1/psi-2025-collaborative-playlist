using Microsoft.AspNetCore.Mvc;
using MyApi.Dtos;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylists()
        {
            var playlists = await _playlistService.GetAllAsync();
            return Ok(playlists);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPlaylistById(int id)
        {
            var playlist = await _playlistService.GetByIdAsync(id);

            if (playlist == null)
                return NotFound($"Playlist with ID {id} not found.");

            return Ok(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistCreateDto newPlaylist)
        {
            var (success, error, created) = await _playlistService.CreateAsync(newPlaylist);

            if (!success)
                return BadRequest(error);

            return CreatedAtAction(nameof(GetPlaylistById), new { id = created!.Id }, created);
        }

        [HttpPut("by-id/{id:int}")]
        public async Task<IActionResult> UpdatePlaylistById(int id, [FromBody] PlaylistUpdateDto updatedPlaylist)
        {
            var (success, error, updated) = await _playlistService.UpdateByIdAsync(id, updatedPlaylist);

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

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> EditPlaylist(int id, [FromBody] PlaylistPatchDto editedPlaylist)
        {
            var (success, error, updated) = await _playlistService.EditAsync(id, editedPlaylist);

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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePlaylistById(int id)
        {
            var (success, error) = await _playlistService.DeleteAsync(id);

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
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var (success, error) = await _playlistService.RemoveSongFromPlaylistAsync(playlistId, songId);

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