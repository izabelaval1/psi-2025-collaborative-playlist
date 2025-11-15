using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;
using MyApi.Repositories;
using Xunit;

namespace TestProject
{
    public class PlaylistRepositoryTests
    {
        // ✅ ADD THIS METHOD
        private PlaylistAppContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<PlaylistAppContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new PlaylistAppContext(options);
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_ShouldReturnPlaylistWithRelations()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var repository = new PlaylistRepository(context);
            
            var user = new User 
            { 
                Id = 1, 
                Username = "host", 
                Role = UserRole.Host,
                PasswordHash = "hashed_password_123"
            };
            
            var playlist = new Playlist 
            { 
                Id = 1, 
                Name = "Test", 
                HostId = 1,
                Host = user
            };
            
            context.Users.Add(user);
            context.Playlists.Add(playlist);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdWithDetailsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
            Assert.NotNull(result.Host);
            Assert.Equal("host", result.Host.Username);
        }
    }
}