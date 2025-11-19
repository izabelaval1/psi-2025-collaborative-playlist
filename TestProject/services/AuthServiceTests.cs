using Xunit;
using Moq;
using MyApi.Services;
using MyApi.Repositories;
using MyApi.Models;
using MyApi.Dtos;
using System.Threading.Tasks;

namespace TestProject
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task Login_ShouldFail_WhenPasswordIsWrong()
        {
            // --- ARRANGE ---
            var mockUserRepo = new Mock<IUserRepository>();
            var mockTokenService = new Mock<ITokenService>();

            // Simulate a stored user in the "database"
            var storedUser = new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                Role = UserRole.Host
            };

            // Setup the mock repository to return our stored user when queried by username
            mockUserRepo.Setup(x => x.GetByUsernameAsync("testuser"))
                        .ReturnsAsync(storedUser);

            // Create an instance of AuthService with mocked dependencies
            var authService = new AuthService(mockUserRepo.Object, mockTokenService.Object);

            // Create a DTO representing the user trying to log in with the WRONG password
            var loginDto = new LoginUserDto
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            // --- ACT ---
            // Call the method under test
            var (success, error, result) = await authService.LoginAsync(loginDto);

            // --- ASSERT ---
            // Check that login failed
            Assert.False(success);
            // Check that the correct error message is returned
            Assert.Equal("Invalid credentials", error);
            // There should be no token returned
            Assert.Null(result);

            // Ensure the TokenService's Generate method was never called
            mockTokenService.Verify(x => x.Generate(It.IsAny<User>()), Times.Never);
        }
        
        [Fact]
        public async Task Login_ShouldSucceed_WhenCredentialsAreCorrect()
        {
            // --- ARRANGE ---
            var mockUserRepo = new Mock<IUserRepository>();
            var mockTokenService = new Mock<ITokenService>();

            // Simulate stored user with hashed password
            var storedUser = new User
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                Role = UserRole.Host
            };

            // Mock GetByUsernameAsync to return our stored user
            mockUserRepo.Setup(x => x.GetByUsernameAsync("testuser"))
                        .ReturnsAsync(storedUser);

            // Mock token generation to return a fake token string
            mockTokenService.Setup(x => x.Generate(storedUser))
                            .Returns("fake-jwt-token");

            // AuthService instance with mocks
            var authService = new AuthService(mockUserRepo.Object, mockTokenService.Object);

            // DTO representing correct login credentials
            var loginDto = new LoginUserDto
            {
                Username = "testuser",
                Password = "correctpassword"
            };

            // --- ACT ---
            var (success, error, result) = await authService.LoginAsync(loginDto);

            // --- ASSERT ---
            // Login should succeed
            Assert.True(success);
            // No error message should be returned
            Assert.Null(error);
            // Result DTO should not be null
            Assert.NotNull(result);
            // Token in result should match what we mocked
            Assert.Equal("fake-jwt-token", result.Token);
            // User information should be correctly returned
            Assert.Equal("testuser", result.User.Username);

            // Verify that token was generated exactly once
            mockTokenService.Verify(x => x.Generate(storedUser), Times.Once);
        }
    }
}
