using MyApi.Dtos;


namespace MyApi.Services
{
    public interface ISpotifyService
    {
        Task<(bool Success, string? Error, string? JsonResult)> SearchTracks(string query);

        Task<(bool Success, string? Error, SpotifyTrackDetails? TrackDetails)> GetTrackDetails(string spotifyId);

        string GenerateLoginUrl();

        Task<SpotifyTokenResult> ExchangeCodeForToken(string code);

        Task<SpotifyTokenResult> RefreshAccessToken(string refreshToken);

    }
}
