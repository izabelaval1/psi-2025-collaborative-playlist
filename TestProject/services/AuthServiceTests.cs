using Xunit; // xUnit framework for writing tests
using Moq; // Moq library to create fake/mock implementations of dependencies
using MyApi.Services; // Your AuthService to test
using MyApi.Repositories; // IUserRepository interface
using MyApi.Models; // User, UserRole
using MyApi.Dtos; // LoginUserDto, LoginResponseDto
using System.Threading.Tasks; // For async/await support

namespace TestProject
{
    // This class contains all tests for the AuthService
    public class AuthServiceTests
    {
        /*
         * Test #1: Login fails when the password is incorrect
         * Key concept: negative scenario testing
         * - We simulate a user in the database
         * - Provide the wrong password in login
         * - Ensure AuthService returns failure
         * - Ensure TokenService.Generate is NOT called
         */
        [Fact] // Marks this method as a test case for xUnit
        public async Task Login_ShouldFail_WhenPasswordIsWrong()
        {
            // --- ARRANGE ---
            // Create mock objects for the dependencies of AuthService
            // We don't want to hit a real database or generate real JWTs
            var mockUserRepo = new Mock<IUserRepository>();
            var mockTokenService = new Mock<ITokenService>();

            // Simulate a stored user in the "database"
            // Notice we hash the password like the real backend does
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
            // This is important: failed login should NOT issue tokens
            mockTokenService.Verify(x => x.Generate(It.IsAny<User>()), Times.Never);
        }

        /*
         * Test #2: Login succeeds when credentials are correct
         * Key concept: positive scenario testing
         * - Provide correct username and password
         * - Ensure AuthService returns success
         * - Ensure TokenService.Generate is called exactly once
         * - Ensure returned DTO has correct information
         */
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
