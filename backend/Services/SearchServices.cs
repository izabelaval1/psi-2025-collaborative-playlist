using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MyApi.Services
{
    public class SpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SpotifyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Paieška Spotify
        // task<...> - async metodas, grąžina rezultatą ateityje
        public async Task<(bool Success, string? Error, string? JsonResult)> SearchTracks(string query)
        {
            try
            {
                // Gauti Spotify credentials iš appsettings.json
                var clientId = _configuration["Spotify:ClientID"];
                var clientSecret = _configuration["Spotify:ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                    return (false, "Spotify credentials not configured", null);

                //  Gauti token
                // kvieciame helper metoda
                var token = await GetSpotifyToken(clientId, clientSecret);
                if (token == null)
                    return (false, "Failed to get Spotify access token", null);

                //  Ieškoti dainų naudojant token
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=5";
                var response = await _httpClient.GetAsync(searchUrl);

                // Grąžinti rezultatą
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return (true, null, json);
                }
                else
                {
                    return (false, $"Spotify API returned status code: {response.StatusCode}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Search failed: {ex.Message}", null);
            }
        }

        // Helper metodas: gauti Spotify access token
        private async Task<string?> GetSpotifyToken(string clientId, string clientSecret)
        {
            try
            {
                using var client = new HttpClient();

                // Užkoduoti credentials
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // Prašyti token iš Spotify
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
                {
                    Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    })
                };

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return null;

                // Ištraukti token iš atsakymo (paprastas string parsing)
                var tokenStart = responseBody.IndexOf("\"access_token\":\"") + 16;
                if (tokenStart < 16)
                    return null;

                var tokenEnd = responseBody.IndexOf("\"", tokenStart);
                if (tokenEnd < 0)
                    return null;

                return responseBody.Substring(tokenStart, tokenEnd - tokenStart);
            }
            catch
            {
                return null;
            }
        }
    }
}