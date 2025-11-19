using Xunit;
using Moq;
using MyApi.Services;
using MyApi.Repositories;
using MyApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestProject.Services
{
    public class CollaborativePlaylistServiceTests
    {
        private readonly Mock<IPlaylistRepository> _playlistRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ISongRepository> _songRepoMock;

        private readonly CollaborativePlaylistService _service;

        public CollaborativePlaylistServiceTests()
        {
            _playlistRepoMock = new Mock<IPlaylistRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _songRepoMock = new Mock<ISongRepository>();

            _service = new CollaborativePlaylistService(
                _playlistRepoMock.Object,
                _userRepoMock.Object,
                _songRepoMock.Object
            );
        }

        // ---------------------------
        // ADD COLLABORATOR
        // ---------------------------

        [Fact]
        public async Task AddCollaboratorAsync_ShouldAdd_WhenHostAddsValidUser()
        {
            var playlist = new Playlist
            {
                Id = 1,
                HostId = 10,
                Users = new List<User>()
            };

            var user = new User { Id = 20 };

            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);
            _userRepoMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(user);

            var result = await _service.AddCollaboratorAsync(1, 20, 10);

            Assert.True(result.Success);
            Assert.Contains(user, playlist.Users);
        }

        [Fact]
        public async Task AddCollaboratorAsync_ShouldFail_WhenRequesterIsNotHost()
        {
            var playlist = new Playlist
            {
                Id = 1,
                HostId = 10,
                Users = new List<User>()
            };

            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);

            var result = await _service.AddCollaboratorAsync(1, 20, 99);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task AddCollaboratorAsync_ShouldFail_WhenPlaylistNotFound()
        {
            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1))
                .ReturnsAsync((Playlist)null);

            var result = await _service.AddCollaboratorAsync(1, 20, 10);

            Assert.False(result.Success);
        }

        // ---------------------------
        // REMOVE COLLABORATOR
        // ---------------------------

        [Fact]
        public async Task RemoveCollaboratorAsync_ShouldRemove_WhenHostRemovesUser()
        {
            var user = new User { Id = 20 };

            var playlist = new Playlist
            {
                Id = 1,
                HostId = 10,
                Users = new List<User> { user }
            };

            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);

            var result = await _service.RemoveCollaboratorAsync(1, 20, 10);

            Assert.True(result.Success);
            Assert.Empty(playlist.Users);
        }

        [Fact]
        public async Task RemoveCollaboratorAsync_ShouldFail_WhenUserNotFound()
        {
            var playlist = new Playlist
            {
                Id = 1,
                HostId = 10,
                Users = new List<User>()
            };

            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);

            var result = await _service.RemoveCollaboratorAsync(1, 20, 10);

            Assert.False(result.Success);
        }

        // ---------------------------
        // CAN ACCESS PLAYLIST
        // ---------------------------

        [Fact]
        public async Task CanAccessPlaylistAsync_ShouldReturnTrue_ForHost()
        {
            var playlist = new Playlist
            {
                Id = 1,
                HostId = 10
            };

            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);

            Assert.True(await _service.CanAccessPlaylistAsync(1, 10));
        }

        [Fact]
        public async Task CanAccessPlaylistAsync_ShouldReturnTrue_ForCollaborator()
        {
            var playlist = new Playlist
            {
                Id = 1,
                HostId = 10,
                Users = new List<User> { new User { Id = 20 } }
            };

            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);

            Assert.True(await _service.CanAccessPlaylistAsync(1, 20));
        }

        [Fact]
        public async Task CanAccessPlaylistAsync_ShouldReturnFalse_ForUnauthorizedUser()
        {
            var playlist = new Playlist
            {
                Id = 1,
                HostId = 10
            };

            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);

            Assert.False(await _service.CanAccessPlaylistAsync(1, 99));
        }

        // ---------------------------
        // ADD SONG
        // ---------------------------

        [Fact]
        public async Task AddSongAsync_ShouldFail_WhenSongAlreadyExists()
        {
            var playlist = new Playlist
            {
                Id = 1,
                HostId = 1,
                PlaylistSongs = new List<PlaylistSong> { new PlaylistSong { SongId = 5 } }
            };

            _playlistRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);
            _songRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Song { Id = 5 });

            var result = await _service.AddSongAsync(1, 5, 1);

            Assert.False(result.Success);
        }


        [Fact]
        public async Task AddCollaboratorAsync_ShouldFail_WhenUserDoesNotExist()
        {
            // Arrange
            var playlist = new Playlist { Id = 1, HostId = 10, Users = new List<User>() };

            var playlistRepo = new Mock<IPlaylistRepository>();
            playlistRepo.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(playlist);

            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User)null); // non-existent user

            var songRepo = new Mock<ISongRepository>();

            var service = new CollaborativePlaylistService(playlistRepo.Object, userRepo.Object, songRepo.Object);

            // Act
            var result = await service.AddCollaboratorAsync(1, 99, 10);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("not found", result.Error);
        }


    }
}
