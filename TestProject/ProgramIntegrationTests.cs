using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace TestProject
{
    public class ProgramIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProgramIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetPlaylists_ShouldReturn200()
        {
            // Act
            var response = await _client.GetAsync("/api/playlists");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetPlaylistById_WithNonExistentId_ShouldReturn404()
        {
            // Act
            var response = await _client.GetAsync("/api/playlists/99999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreatePlaylist_WithInvalidData_ShouldReturn400()
        {
            // Act - Send invalid data (missing required fields)
            var response = await _client.PostAsJsonAsync("/api/playlists", new { });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}