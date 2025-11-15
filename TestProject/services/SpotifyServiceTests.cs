using Moq;
using MyApi.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace TestProject.Services
{
    public class SpotifyServiceTests
    {
        [Fact]
        public async Task SearchTracks_WithMissingCredentials_ShouldReturnError()
        {
            // --- ARRANGE ---
            var mockHttpClient = new HttpClient();
            var mockConfiguration = new Mock<IConfiguration>();

            // Mock missing credentials
            mockConfiguration.Setup(x => x["Spotify:ClientID"]).Returns((string?)null);
            mockConfiguration.Setup(x => x["Spotify:ClientSecret"]).Returns((string?)null);

            var service = new SpotifyService(mockHttpClient, mockConfiguration.Object);

            // --- ACT ---
            var (success, error, jsonResult) = await service.SearchTracks("test query");

            // --- ASSERT ---
            Assert.False(success);
            Assert.Equal("Spotify credentials not configured", error);
            Assert.Null(jsonResult);
        }

        [Fact]
        public async Task SearchTracks_WithOnlyClientIdMissing_ShouldReturnError()
        {
            // --- ARRANGE ---
            var mockHttpClient = new HttpClient();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Spotify:ClientID"]).Returns((string?)null);
            mockConfiguration.Setup(x => x["Spotify:ClientSecret"]).Returns("test_secret");

            var service = new SpotifyService(mockHttpClient, mockConfiguration.Object);

            // --- ACT ---
            var (success, error, jsonResult) = await service.SearchTracks("test");

            // --- ASSERT ---
            Assert.False(success);
            Assert.Equal("Spotify credentials not configured", error);
            Assert.Null(jsonResult);
        }

        [Fact]
        public async Task SearchTracks_WithOnlyClientSecretMissing_ShouldReturnError()
        {
            // --- ARRANGE ---
            var mockHttpClient = new HttpClient();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Spotify:ClientID"]).Returns("test_id");
            mockConfiguration.Setup(x => x["Spotify:ClientSecret"]).Returns((string?)null);

            var service = new SpotifyService(mockHttpClient, mockConfiguration.Object);

            // --- ACT ---
            var (success, error, jsonResult) = await service.SearchTracks("test");

            // --- ASSERT ---
            Assert.False(success);
            Assert.Equal("Spotify credentials not configured", error);
            Assert.Null(jsonResult);
        }

        [Fact]
        public async Task SearchTracks_WithEmptyClientId_ShouldReturnError()
        {
            // --- ARRANGE ---
            var mockHttpClient = new HttpClient();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Spotify:ClientID"]).Returns("");
            mockConfiguration.Setup(x => x["Spotify:ClientSecret"]).Returns("test_secret");

            var service = new SpotifyService(mockHttpClient, mockConfiguration.Object);

            // --- ACT ---
            var (success, error, jsonResult) = await service.SearchTracks("test");

            // --- ASSERT ---
            Assert.False(success);
            Assert.Equal("Spotify credentials not configured", error);
            Assert.Null(jsonResult);
        }

        [Fact]
        public async Task SearchTracks_WithEmptyClientSecret_ShouldReturnError()
        {
            // --- ARRANGE ---
            var mockHttpClient = new HttpClient();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(x => x["Spotify:ClientID"]).Returns("test_id");
            mockConfiguration.Setup(x => x["Spotify:ClientSecret"]).Returns("");

            var service = new SpotifyService(mockHttpClient, mockConfiguration.Object);

            // --- ACT ---
            var (success, error, jsonResult) = await service.SearchTracks("test");

            // --- ASSERT ---
            Assert.False(success);
            Assert.Equal("Spotify credentials not configured", error);
            Assert.Null(jsonResult);
        }
    }
}