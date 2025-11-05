using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using MyApi.Exceptions;

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
        // Uses async/await to perform non-blocking Spotify API calls
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
                    throw new SpotifyServiceException("Failed to get Spotify access token.");


                //  Ieškoti dainų naudojant token
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=5";
                var response = await _httpClient.GetAsync(searchUrl);

                // Grąžinti rezultatą
                if (response.IsSuccessStatusCode) // jei Spotify API atsakymas sekmingas (HTTP kodas 200-299)
                {
                    // Nuskaitom Spotify atsakymą kaip srautą (stream)
                    // response.Content - objektas kuriame saugomas Spotify ats
                    // ReadAsStreamAsync() - asinchroninis metodas, kuris nuskaito turinį kaip srautą
                    // await using - užtikrina, kad stream bus tinkamai uždarytas po naudojimo
                    await using var stream = await response.Content.ReadAsStreamAsync();

                    // Naudojam StreamReader, kad paverstume stream i teksta
                    // StreamReader - klase skirta skaityti teksta iš srauto, gauna baitus ir paverčia juos į string
                    using var reader = new StreamReader(stream);
                    // nuskaitom visą tekstą iš stream  ir grazinam string su ReadToEndAsync()
                    var json = await reader.ReadToEndAsync();

                    // asinchroninis metodas grąžina tuple su sekmės būsena, klaidos žinute (jei yra) ir JSON rezultatu
                    return (true, null, json);
                }
                else
                {
                    throw new SpotifyServiceException($"Spotify API returned status code: {response.StatusCode}");
                }

            }
            catch (SpotifyServiceException ex)
            {
                // Log to file
                File.AppendAllText("logs.txt", $"{DateTime.Now}: {ex.Message}\n");
                return (false, ex.Message, null);
            }
            catch (Exception ex)
            {
                // visos kitos netikėtos klaidos
                File.AppendAllText("logs.txt", $"{DateTime.Now}: Unexpected error: {ex.Message}\n");
                return (false, $"Unexpected error: {ex.Message}", null);
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