using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/playlists")]
    public class CollaborativePlaylistController : ControllerBase
    {
        private readonly ICollaborativePlaylistService _collaborativeService;

        public CollaborativePlaylistController(ICollaborativePlaylistService collaborativeService)
        {
            _collaborativeService = collaborativeService;
        }
        
        // GET: api/playlists/{playlistId}/collaborators
        // Gauti visus playlisto collaborators
        [HttpGet("{playlistId}/collaborators")]
        public async Task<IActionResult> GetCollaborators(int playlistId)
        {
            var collaborators = await _collaborativeService.GetCollaboratorsAsync(playlistId);

            if (collaborators == null)
                return NotFound(new { message = $"Playlist with ID {playlistId} not found." });

            return Ok(collaborators);
        }

        // POST: api/playlists/{playlistId}/collaborators
        // Pridėti kolaboratorių pagal username (tik host)
        // Body: { "username": "john_doe" }
        [Authorize]
        [HttpPost("{playlistId}/collaborators")]
        public async Task<IActionResult> AddCollaborator(
            int playlistId,
            [FromBody] AddCollaboratorByUsernameRequest request)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int requesterId))
            {
                return Unauthorized("Invalid token");
            }

            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest(new { message = "Username is required." });

            var (success, error) = await _collaborativeService.AddCollaboratorByUsernameAsync(
                playlistId,
                request.Username,
                requesterId);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Collaborator added successfully." });
        }

        // DELETE: api/playlists/{playlistId}/collaborators/{userId}
        // Pašalinti kolaboratorių (tik host)
        [Authorize]
        [HttpDelete("{playlistId}/collaborators/{userId}")]
        public async Task<IActionResult> RemoveCollaborator(
            int playlistId,
            int userId)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int requesterId))
            {
                return Unauthorized("Invalid token");
            }

            var (success, error) = await _collaborativeService.RemoveCollaboratorAsync(
                playlistId,
                userId,
                requesterId);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Collaborator removed successfully." });
        }

        // POST: api/playlists/{playlistId}/songs
        // Pridėti dainą į grojaraštį (host arba collaborator)
        // Body: { "songId": 10 }
        [Authorize]
        [HttpPost("{playlistId}/songs")]
        public async Task<IActionResult> AddSongToPlaylist(
            int playlistId,
            [FromBody] AddSongToCollaborativePlaylistRequest request)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            if (request.SongId <= 0)
                return BadRequest(new { message = "Invalid song ID." });

            var (success, error) = await _collaborativeService.AddSongAsync(
                playlistId,
                request.SongId,
                userId);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Song added to playlist successfully." });
        }

        // DELETE: api/playlists/{playlistId}/songs/{songId}
        // Pašalinti dainą iš grojaraščio (host arba collaborator)
        [Authorize]
        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(
            int playlistId,
            int songId)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            var (success, error) = await _collaborativeService.RemoveSongAsync(
                playlistId,
                songId,
                userId);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Song removed from playlist successfully." });
        }

        // GET: api/playlists/{playlistId}/access
        // Patikrinti, ar vartotojas turi prieigą prie grojaraščio
        [Authorize]
        [HttpGet("{playlistId}/access")]
        public async Task<IActionResult> CheckAccess(int playlistId)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            var hasAccess = await _collaborativeService.CanAccessPlaylistAsync(playlistId, userId);

            return Ok(new { hasAccess });
        }

        // POST: api/playlists/{playlistId}/session/join
        [Authorize]
        [HttpPost("{playlistId}/session/join")]
        public async Task<IActionResult> JoinSession(int playlistId)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            await _collaborativeService.JoinPlaylistSessionAsync(playlistId, userId);
            return Ok(new { message = "Joined playlist session." });
        }

        // POST: api/playlists/{playlistId}/session/leave
        [Authorize]
        [HttpPost("{playlistId}/session/leave")]
        public async Task<IActionResult> LeaveSession(int playlistId)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            await _collaborativeService.LeavePlaylistSessionAsync(playlistId, userId);
            return Ok(new { message = "Left playlist session." });
        }

        // GET: api/playlists/{playlistId}/active-users
        [HttpGet("{playlistId}/active-users")]
        public async Task<IActionResult> GetActiveUsers(int playlistId)
        {
            var activeUsers = await _collaborativeService.GetActiveUsersAsync(playlistId);
            return Ok(activeUsers);
        }
    }

    // Request DTOs
    public class AddCollaboratorByUsernameRequest
    {
        public string Username { get; set; } = null!;
    }

    public class AddCollaboratorRequest
    {
        public int UserId { get; set; }
        public int RequesterId { get; set; }
    }

    public class AddSongToCollaborativePlaylistRequest
    {
        public int SongId { get; set; }
    }

    public class AddSongRequest
    {
        public int SongId { get; set; }
        public int UserId { get; set; }
    }
}