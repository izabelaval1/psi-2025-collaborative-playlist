using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpotifyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        // Constructor - ASP.NET gives us an HttpClient to make web requests
        public SpotifyController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpGet("search/{query}")]
        public async Task<IActionResult> Search(string query) // IActionresult means that it returns an http response. Async functions can be Task<> or void
                                                              // Async means that it will return sth in the future whan it's done and not right away
        {
            var clientId = _configuration["Spotify:ClientID"];          //here you need to enter your generated client id
            var clientSecret = _configuration["Spotify:ClientSecret"];  //and client secret that you got from spotify api
                                                                        //in your appsettings.json file

            // Step 1: Get permission from Spotify (access token)
            var token = await GetSpotifyToken(clientId, clientSecret); //from credentials we need to get a token that expires every 30 mins or so 

            // Step 2: Search for tracks using our token
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=5";
            var response = await _httpClient.GetAsync(searchUrl);

            // Step 3: Return what Spotify gave us
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json"); // Return raw JSON
            }
            else
            {
                return StatusCode(500, "Spotify search failed");
            }
        }

        // Helper method: Ask Spotify for permission (access token)
        private async Task<string> GetSpotifyToken(string clientId, string clientSecret)
        {
            using var client = new HttpClient();

            // Encode our credentials
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            // Ask Spotify for a token
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                })
            };

            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Extract token from response (simple string parsing)
            var tokenStart = responseBody.IndexOf("\"access_token\":\"") + 16;
            var tokenEnd = responseBody.IndexOf("\"", tokenStart);
            return responseBody.Substring(tokenStart, tokenEnd - tokenStart);
        }
    }
}