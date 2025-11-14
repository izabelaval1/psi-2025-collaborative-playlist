namespace MyApi.Services
{
    public interface ISpotifyService
    {
        Task<(bool Success, string? Error, string? JsonResult)> SearchTracks(string query);
    }
}
