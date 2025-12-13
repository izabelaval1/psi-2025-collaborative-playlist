using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Services;
using MyApi.Dtos;
using MyApi.Data;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpotifyAuthController : ControllerBase
    {
        private readonly ISpotifyService _spotify;
        private readonly PlaylistAppContext _db;

        public SpotifyAuthController(
            ISpotifyService spotify,
            PlaylistAppContext db
        )
        {
            _spotify = spotify;
            _db = db;
        }

        // public SpotifyAuthController(ISpotifyService spotify)
        //     => _spotify = spotify;

        [HttpGet("login-url")]
        public IActionResult GetLoginUrl()
        {
            var url = _spotify.GenerateLoginUrl();
            return Ok(new { url });
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] SpotifyCallbackDto dto)
        {
            var result = await _spotify.ExchangeCodeForToken(dto.Code);
            if (!result.Success)
                return BadRequest(new { error = result.Error });

            return Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken))
                return BadRequest(new { error = "Refresh token is required" });

            var result = await _spotify.RefreshAccessToken(dto.RefreshToken);

            if (!result.Success)
                return Unauthorized(new { error = result.Error });

            // Optionally update database if you're storing tokens per user
            // var user = await _db.Users.FirstOrDefaultAsync();
            // if (user != null)
            // {
            //     user.SpotifyAccessToken = result.AccessToken;
            //     await _db.SaveChangesAsync();
            // }

            return Ok(new { accessToken = result.AccessToken });
        }

    }
}
