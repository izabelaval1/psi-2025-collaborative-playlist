using Microsoft.AspNetCore.Mvc;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/spotify")] // ⚠️ Pataisyta - dabar bus /api/spotify/search/{query}
    public class SpotifyController : ControllerBase
    {
        private readonly SpotifyService _spotifyService;

        public SpotifyController(SpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        // GET /api/spotify/search/{query}
        [HttpGet("search/{query}")]
        public async Task<IActionResult> Search(string query)
        {
            var (success, error, jsonResult) = await _spotifyService.SearchTracks(query);

            if (!success)
                return StatusCode(500, error ?? "Spotify search failed");

            return Content(jsonResult!, "application/json");
        }
    }
}