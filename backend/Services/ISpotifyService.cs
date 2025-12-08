using MyApi.Dtos; // <-- ADD THIS LINE

namespace MyApi.Services
{
    public interface ISpotifyService
    {
        Task<(bool Success, string? Error, string? JsonResult)> SearchTracks(string query);

        // This line requires the using statement
        Task<(bool Success, string? Error, SpotifyTrackDetails? TrackDetails)> GetTrackDetails(string spotifyId);
    }
}