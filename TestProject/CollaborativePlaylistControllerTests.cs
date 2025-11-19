using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers;
using MyApi.Services;
using MyApi.Models;
using MyApi.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject.Controllers
{
    public class CollaborativePlaylistControllerTests
    {
        private readonly Mock<ICollaborativePlaylistService> _serviceMock;
        private readonly CollaborativePlaylistController _controller;

        public CollaborativePlaylistControllerTests()
        {
            _serviceMock = new Mock<ICollaborativePlaylistService>();
            _controller = new CollaborativePlaylistController(_serviceMock.Object);
        }

        // ---------------------------
        // GET COLLABORATORS
        // ---------------------------

        [Fact]
        public async Task GetCollaborators_ReturnsOk_WhenPlaylistExists()
        {
            var collaborators = new List<UserDto> { new UserDto { Id = 1, Username = "user1" } };

            _serviceMock.Setup(s => s.GetCollaboratorsAsync(1))
                        .ReturnsAsync((IEnumerable<UserDto>)collaborators);

            var result = await _controller.GetCollaborators(1) as OkObjectResult;

            Assert.NotNull(result);
            var users = Assert.IsAssignableFrom<IEnumerable<UserDto>>(result.Value);
            Assert.Single(users);
        }

        [Fact]
        public async Task GetCollaborators_ReturnsNotFound_WhenPlaylistDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetCollaboratorsAsync(1))
                        .ReturnsAsync((IEnumerable<UserDto>?)null);

            var result = await _controller.GetCollaborators(1) as NotFoundObjectResult;

            Assert.NotNull(result);
        }

        // ---------------------------
        // ADD COLLABORATOR
        // ---------------------------

        [Fact]
        public async Task AddCollaborator_ReturnsOk_WhenSuccess()
        {
            _serviceMock.Setup(s => s.AddCollaboratorAsync(1, 2, 1))
                        .ReturnsAsync((true, null!));

            var request = new AddCollaboratorRequest { UserId = 2, RequesterId = 1 };
            var result = await _controller.AddCollaborator(1, request) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddCollaborator_ReturnsBadRequest_WhenFailure()
        {
            _serviceMock.Setup(s => s.AddCollaboratorAsync(1, 2, 1))
                        .ReturnsAsync((false, "Error message"));

            var request = new AddCollaboratorRequest { UserId = 2, RequesterId = 1 };
            var result = await _controller.AddCollaborator(1, request) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddCollaborator_ReturnsBadRequest_ForInvalidIds()
        {
            var request = new AddCollaboratorRequest { UserId = 0, RequesterId = -1 };
            var result = await _controller.AddCollaborator(1, request) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        // ---------------------------
        // REMOVE COLLABORATOR
        // ---------------------------

        [Fact]
        public async Task RemoveCollaborator_ReturnsOk_WhenSuccess()
        {
            _serviceMock.Setup(s => s.RemoveCollaboratorAsync(1, 2, 1))
                        .ReturnsAsync((true, null!));

            var result = await _controller.RemoveCollaborator(1, 2, 1) as OkObjectResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveCollaborator_ReturnsBadRequest_WhenFailure()
        {
            _serviceMock.Setup(s => s.RemoveCollaboratorAsync(1, 2, 1))
                        .ReturnsAsync((false, "Error message"));

            var result = await _controller.RemoveCollaborator(1, 2, 1) as BadRequestObjectResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveCollaborator_ReturnsBadRequest_ForInvalidRequesterId()
        {
            var result = await _controller.RemoveCollaborator(1, 2, 0) as BadRequestObjectResult;
            Assert.NotNull(result);
        }

        // ---------------------------
        // ADD SONG
        // ---------------------------

        [Fact]
        public async Task AddSongToPlaylist_ReturnsOk_WhenSuccess()
        {
            _serviceMock.Setup(s => s.AddSongAsync(1, 2, 1))
                        .ReturnsAsync((true, null!));

            var request = new AddSongRequest { SongId = 2, UserId = 1 };
            var result = await _controller.AddSongToPlaylist(1, request) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddSongToPlaylist_ReturnsBadRequest_WhenFailure()
        {
            _serviceMock.Setup(s => s.AddSongAsync(1, 2, 1))
                        .ReturnsAsync((false, "Error message"));

            var request = new AddSongRequest { SongId = 2, UserId = 1 };
            var result = await _controller.AddSongToPlaylist(1, request) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddSongToPlaylist_ReturnsBadRequest_ForInvalidIds()
        {
            var request = new AddSongRequest { SongId = 0, UserId = 0 };
            var result = await _controller.AddSongToPlaylist(1, request) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        // ---------------------------
        // REMOVE SONG
        // ---------------------------

        [Fact]
        public async Task RemoveSongFromPlaylist_ReturnsOk_WhenSuccess()
        {
            _serviceMock.Setup(s => s.RemoveSongAsync(1, 2, 1))
                        .ReturnsAsync((true, null!));

            var result = await _controller.RemoveSongFromPlaylist(1, 2, 1) as OkObjectResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveSongFromPlaylist_ReturnsBadRequest_WhenFailure()
        {
            _serviceMock.Setup(s => s.RemoveSongAsync(1, 2, 1))
                        .ReturnsAsync((false, "Error message"));

            var result = await _controller.RemoveSongFromPlaylist(1, 2, 1) as BadRequestObjectResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveSongFromPlaylist_ReturnsBadRequest_ForInvalidUserId()
        {
            var result = await _controller.RemoveSongFromPlaylist(1, 2, 0) as BadRequestObjectResult;
            Assert.NotNull(result);
        }

        // ---------------------------
        // CHECK ACCESS
        // ---------------------------

        [Fact]
        public async Task CheckAccess_ReturnsOk_WithHasAccess()
        {
            _serviceMock.Setup(s => s.CanAccessPlaylistAsync(1, 1))
                        .ReturnsAsync(true);

            var result = await _controller.CheckAccess(1, 1) as OkObjectResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CheckAccess_ReturnsBadRequest_ForInvalidUserId()
        {
            var result = await _controller.CheckAccess(1, 0) as BadRequestObjectResult;
            Assert.NotNull(result);
        }

        // ---------------------------
        // JOIN / LEAVE SESSION
        // ---------------------------

        [Fact]
        public async Task JoinSession_ReturnsOk_ForValidUser()
        {
            var result = await _controller.JoinSession(1, 1) as OkObjectResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task JoinSession_ReturnsBadRequest_ForInvalidUser()
        {
            var result = await _controller.JoinSession(1, 0) as BadRequestObjectResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task LeaveSession_ReturnsOk_ForValidUser()
        {
            var result = await _controller.LeaveSession(1, 1) as OkObjectResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task LeaveSession_ReturnsBadRequest_ForInvalidUser()
        {
            var result = await _controller.LeaveSession(1, 0) as BadRequestObjectResult;
            Assert.NotNull(result);
        }

        // ---------------------------
        // GET ACTIVE USERS
        // ---------------------------

        [Fact]
        public async Task GetActiveUsers_ReturnsOk()
        {
            var activeUsers = new List<ActiveUserDto> { new ActiveUserDto { UserId = 1 } };

            _serviceMock.Setup(s => s.GetActiveUsersAsync(1))
                        .ReturnsAsync((IEnumerable<ActiveUserDto>)activeUsers);

            var result = await _controller.GetActiveUsers(1) as OkObjectResult;

            Assert.NotNull(result);
            var users = Assert.IsAssignableFrom<IEnumerable<ActiveUserDto>>(result.Value);
            Assert.Single(users);
        }
    }
}
