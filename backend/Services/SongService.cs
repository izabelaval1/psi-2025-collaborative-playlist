using System.Text.Json;
using MyApi.Models;
using MyApi.Dtos;
using MyApi.Utils;
using MyApi.Repositories;
using MyApi.Services;

namespace MyApi.Services
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ISpotifyService _spotifyService;
        private readonly GenericConverter<Song, SongDto> _converter;

        public SongService(
            ISongRepository songRepository,
            IPlaylistRepository playlistRepository,
            ISpotifyService spotifyService)
        {
            _songRepository = songRepository;
            _playlistRepository = playlistRepository;
            _spotifyService = spotifyService;
            _converter = new GenericConverter<Song, SongDto>();
        }

        public async Task<IEnumerable<SongDto>> GetAllAsync()
        {
            var entities = (await _songRepository.GetAllAsync()).ToList();

            return _converter.ConvertAll(entities, s => new SongDto
            {
                Id = s.Id,
                Title = s.Title,
                Album = s.Album,
                DurationMs = s.DurationSeconds.HasValue ? s.DurationSeconds.Value * 1000 : null,
                DurationFormatted = s.DurationSeconds.HasValue ? new Duration(s.DurationSeconds.Value).ToString() : null,
                SpotifyId = s.SpotifyId,
                SpotifyUri = s.SpotifyUri,
                Artists = s.Artists.Select(a => new ArtistDto { Id = a.Id, Name = a.Name }).ToList()
            });
        }

        public async Task<SongDto?> GetByIdAsync(int id)
        {
            var s = await _songRepository.GetByIdAsync(id);
            if (s == null) return null;

            return _converter.ConvertOne(s, x => new SongDto
            {
                Id = x.Id,
                Title = x.Title,
                Album = x.Album,
                DurationMs = x.DurationSeconds.HasValue ? x.DurationSeconds.Value * 1000 : null,
                DurationFormatted = x.DurationSeconds.HasValue ? new Duration(x.DurationSeconds.Value).ToString() : null,
                SpotifyId = x.SpotifyId,
                SpotifyUri = x.SpotifyUri,
                Artists = x.Artists.Select(a => new ArtistDto { Id = a.Id, Name = a.Name }).ToList()
            });
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var s = await _songRepository.GetByIdAsync(id);
            if (s == null) return (false, $"Song with ID {id} not found.");

            await _songRepository.DeleteAsync(s);
            return (true, null);
        }

        public async Task<(bool Success, string? Error, int? SongId)> AddSongToPlaylistAsync(AddSongToPlaylistDto request)
        {
            var playlist = await _playlistRepository.GetByIdWithDetailsAsync(request.PlaylistId);
            if (playlist == null)
                return (false, $"Playlist with ID {request.PlaylistId} not found.", null);
            if (string.IsNullOrEmpty(request.SpotifyId))
            {
                return (false, "SpotifyId is required to add a track.", null);
            }

            // Fetch track details from Spotify
            var (fetchSuccess, fetchError, trackDetails) = await _spotifyService.GetTrackDetails(request.SpotifyId);

            if (!fetchSuccess || trackDetails == null)
            {
                return (false, $"Failed to retrieve song details from Spotify: {fetchError}", null);
            }

            if (string.IsNullOrEmpty(trackDetails.SpotifyId))
            {
                return (false, "Spotify returned invalid track ID.", null);
            }

            if (string.IsNullOrEmpty(trackDetails.SpotifyUri))
            {
                return (false, "Spotify returned invalid track URI.", null);
            }

            // Extract data with null-safety
            var artistNames = trackDetails.ArtistNames ?? new List<string>();
            
            int? durationSeconds = null;
            if (trackDetails.DurationMs.HasValue)
                durationSeconds = (int)(trackDetails.DurationMs.Value / 1000);

            var spotifyId = trackDetails.SpotifyId;
            var spotifyUri = trackDetails.SpotifyUri;
            var album = trackDetails.AlbumInfo?.Name; // Safely access nested property

            // Save to database
            var song = await _songRepository.EnsureSongWithArtistsAsync(
                title: trackDetails.Title,
                album: album,
                durationSeconds: durationSeconds,
                artistNames: artistNames,
                spotifyId: spotifyId,
                spotifyUri: spotifyUri
            );

            // Check if song already in playlist
            var already = playlist.PlaylistSongs.Any(ps => ps.SongId == song.Id);
            if (already)
                return (false, "This song is already in the playlist.", null);

            // Add to playlist
            var nextPos = playlist.PlaylistSongs.NextPosition();
            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlist.Id,
                SongId = song.Id,
                Position = nextPos
            };

            await _playlistRepository.AddPlaylistSongAsync(playlistSong);

            return (true, null, song.Id);
        }
    }
}