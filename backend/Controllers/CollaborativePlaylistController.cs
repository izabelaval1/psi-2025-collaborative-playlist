using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/collaborative")]
    public class CollaborativePlaylistController : ControllerBase
    {
        private readonly ICollaborativePlaylistService _collaborativeService;

        public CollaborativePlaylistController(ICollaborativePlaylistService collaborativeService)
        {
            _collaborativeService = collaborativeService;
        }
        
        //  GET: api/playlists/{playlistId}/collaborators
        //  Gauti visus playlisto collaborators

        [HttpGet("{playlistId}/collaborators")]
        public async Task<IActionResult> GetCollaborators(int playlistId)
        {
            var collaborators = await _collaborativeService.GetCollaboratorsAsync(playlistId);

            if (collaborators == null)
                return NotFound(new { message = $"Playlist with ID {playlistId} not found." });

            return Ok(collaborators);
        }

        //  POST: api/playlists/{playlistId}/collaborators
        //  Pridėti kolaboratorių (tik host)
        //  Body: { "userId": 5, "requesterId": 1 }

        [HttpPost("{playlistId}/collaborators")]
        public async Task<IActionResult> AddCollaborator(
            int playlistId,
            [FromBody] AddCollaboratorRequest request)
        {
            if (request.UserId <= 0 || request.RequesterId <= 0)
                return BadRequest(new { message = "Invalid user IDs." });

            var (success, error) = await _collaborativeService.AddCollaboratorAsync(
                playlistId,
                request.UserId,
                request.RequesterId);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Collaborator added successfully." });
        }


        //  DELETE: api/playlists/{playlistId}/collaborators/{userId}
        //  Pašalinti kolaboratorių (tik host)

        [HttpDelete("{playlistId}/collaborators/{userId}")]
        public async Task<IActionResult> RemoveCollaborator(
            int playlistId,
            int userId,
            [FromQuery] int requesterId)
        {
            if (requesterId <= 0)
                return BadRequest(new { message = "RequesterID is required." });

            var (success, error) = await _collaborativeService.RemoveCollaboratorAsync(
                playlistId,
                userId,
                requesterId);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Collaborator removed successfully." });
        }


        //  POST: api/playlists/{playlistId}/songs
        //  Pridėti dainą į grojaraštį (host arba collaborator)
        //  Body: { "songId": 10, "userId": 5 }

        [HttpPost("{playlistId}/songs")]
        public async Task<IActionResult> AddSongToPlaylist(
            int playlistId,
            [FromBody] AddSongRequest request)
        {
            if (request.SongId <= 0 || request.UserId <= 0)
                return BadRequest(new { message = "Invalid song or user ID." });

            var (success, error) = await _collaborativeService.AddSongAsync(
                playlistId,
                request.SongId,
                request.UserId);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Song added to playlist successfully." });
        }


        //  DELETE: api/playlists/{playlistId}/songs/{songId}
        //  Pašalinti dainą iš grojaraščio (host arba collaborator)
        //  Query: ?userId=5

        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(
            int playlistId,
            int songId,
            [FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "UserID is required." });

            var (success, error) = await _collaborativeService.RemoveSongAsync(
                playlistId,
                songId,
                userId);

            if (!success)
                return BadRequest(new { message = error });

            return Ok(new { message = "Song removed from playlist successfully." });
        }

        //  GET: api/playlists/{playlistId}/access
        //  Patikrinti, ar vartotojas turi prieigą prie grojaraščio
        //  Query: ?userId=5

        [HttpGet("{playlistId}/access")]
        public async Task<IActionResult> CheckAccess(int playlistId, [FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "UserID is required." });

            var hasAccess = await _collaborativeService.CanAccessPlaylistAsync(playlistId, userId);

            return Ok(new { hasAccess });
        }

        // POST: api/collaborative/{playlistId}/session/join
        [HttpPost("{playlistId}/session/join")]
        public async Task<IActionResult> JoinSession(int playlistId, [FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "UserID is required." });

            await _collaborativeService.JoinPlaylistSessionAsync(playlistId, userId);
            return Ok(new { message = "Joined playlist session." });
        }


        // POST: api/collaborative/{playlistId}/session/leave
        [HttpPost("{playlistId}/session/leave")]

        public async Task<IActionResult> LeaveSession(int playlistId, [FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest(new { message = "UserID is required." });

            await _collaborativeService.LeavePlaylistSessionAsync(playlistId, userId);
            return Ok(new { message = "Left playlist session." });
        }

        // GET: api/collaborative/{playlistId}/active-users
        [HttpGet("{playlistId}/active-users")]
        public async Task<IActionResult> GetActiveUsers(int playlistId)
        {
            var activeUsers = await _collaborativeService.GetActiveUsersAsync(playlistId);
            return Ok(activeUsers);
        }
    }
} 


