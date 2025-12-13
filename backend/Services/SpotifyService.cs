// 📁 MyApi/Services/SpotifyService.cs

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json; // Required for deserialization
using MyApi.Exceptions; // Assuming this namespace exists
using MyApi.Dtos;      // <-- REQUIRED FIX for SpotifyTrackDetails
using System.Web;

namespace MyApi.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SpotifyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // EXISTING METHOD (Implementation of SearchTracks)
        public async Task<(bool Success, string? Error, string? JsonResult)> SearchTracks(string query)
        {
            try
            {
                var clientId = _configuration["Spotify:ClientID"];
                var clientSecret = _configuration["Spotify:ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                    return (false, "Spotify credentials not configured", null);

                var token = await GetSpotifyToken(clientId, clientSecret);
                if (token == null)
                    throw new SpotifyServiceException("Failed to get Spotify access token.");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // USING ORIGINAL PLACEHOLDER URL:
                var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=5";
                var response = await _httpClient.GetAsync(searchUrl);

                if (response.IsSuccessStatusCode)
                {
                    await using var stream = await response.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);
                    var json = await reader.ReadToEndAsync();
                    return (true, null, json);
                }
                else
                {
                    throw new SpotifyServiceException($"Spotify API returned status code: {response.StatusCode}");
                }
            }
            catch (SpotifyServiceException ex)
            {
                return (false, ex.Message, null);
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error: {ex.Message}", null);
            }
        }

        // NEW METHOD (Implementation of GetTrackDetails)
        public async Task<(bool Success, string? Error, SpotifyTrackDetails? TrackDetails)> GetTrackDetails(string spotifyId)
        {
            try
            {
                var clientId = _configuration["Spotify:ClientID"];
                var clientSecret = _configuration["Spotify:ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                    return (false, "Spotify credentials not configured", null);

                var token = await GetSpotifyToken(clientId, clientSecret);
                if (token == null)
                    throw new SpotifyServiceException("Failed to get Spotify access token.");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Using new URL placeholder for single track GET request:
                var trackUrl = $"https://api.spotify.com/v1/tracks/{Uri.EscapeDataString(spotifyId)}";
                var response = await _httpClient.GetAsync(trackUrl);

                if (response.IsSuccessStatusCode)
                {
                    await using var stream = await response.Content.ReadAsStreamAsync();

                    var trackDetails = await JsonSerializer.DeserializeAsync<SpotifyTrackDetails>(stream,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (trackDetails == null)
                        return (false, "Failed to parse Spotify track details.", null);

                    return (true, null, trackDetails);
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new SpotifyServiceException($"Spotify API returned status code: {response.StatusCode}. Response: {errorBody}");
                }
            }
            catch (SpotifyServiceException ex)
            {
                return (false, ex.Message, null);
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error fetching track: {ex.Message}", null);
            }
        }


        // Helper metodas: gauti Spotify access token
        private async Task<string?> GetSpotifyToken(string clientId, string clientSecret)
        {
            try
            {
                using var client = new HttpClient();
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // USING ORIGINAL PLACEHOLDER URL:
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


        public string GenerateLoginUrl()
        {
            var clientId = _configuration["Spotify:ClientID"];
            var redirectUri = _configuration["Spotify:RedirectUri"];

            var scope = "streaming user-read-email user-read-private user-read-playback-state user-modify-playback-state";

            var url =
                $"https://accounts.spotify.com/authorize" +
                $"?response_type=code" +
                $"&client_id={clientId}" +
                $"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}" +
                $"&scope={HttpUtility.UrlEncode(scope)}";

            return url;
        }
        public async Task<SpotifyTokenResult> ExchangeCodeForToken(string code)
        {
            var clientId = _configuration["Spotify:ClientID"];
            var clientSecret = _configuration["Spotify:ClientSecret"];
            var redirectUri = _configuration["Spotify:RedirectUri"];

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "authorization_code"},
            {"code", code},
            {"redirect_uri", redirectUri},
            {"client_id", clientId},
            {"client_secret", clientSecret}
        })
            };

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new SpotifyTokenResult { Success = false, Error = json };

            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return new SpotifyTokenResult
            {
                Success = true,
                AccessToken = root.GetProperty("access_token").GetString(),
                RefreshToken = root.GetProperty("refresh_token").GetString()
            };
        }
        public async Task<SpotifyTokenResult> RefreshAccessToken(string refreshToken)
        {
            var clientId = _configuration["Spotify:ClientID"];
            var clientSecret = _configuration["Spotify:ClientSecret"];

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        })
            };

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return new SpotifyTokenResult { Success = false, Error = json };

            var doc = JsonDocument.Parse(json);

            return new SpotifyTokenResult
            {
                Success = true,
                AccessToken = doc.RootElement.GetProperty("access_token").GetString()
            };
        }


    }
}