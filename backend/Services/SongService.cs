using MyApi.Models;
using MyApi.Dtos;
using MyApi.Utils;
using MyApi.Repositories;


namespace MyApi.Services
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly GenericConverter<Song, SongDto> _converter;

        public SongService(ISongRepository songRepository, IPlaylistRepository playlistRepository)
        {
            _songRepository = songRepository;
            _playlistRepository = playlistRepository;
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
                Duration = s.DurationSeconds.HasValue ? s.DurationSeconds.Value * 1000 : null,
                DurationFormatted = s.DurationSeconds.HasValue ? new Duration(s.DurationSeconds.Value).ToString() : null,
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
                Duration = x.DurationSeconds.HasValue ? x.DurationSeconds.Value * 1000 : null,
                DurationFormatted = x.DurationSeconds.HasValue ? new Duration(x.DurationSeconds.Value).ToString() : null,
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

            var artistNames = request.ArtistNames
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .ToList();

            int? durationSeconds = null;
            if (request.DurationMs.HasValue)
                durationSeconds = (int)(request.DurationMs.Value / 1000);

            // užtikriname dainą + atlikėjus per repo
            var song = await _songRepository.EnsureSongWithArtistsAsync(
                title: request.Title,
                album: request.Album,
                durationSeconds: durationSeconds,
                artistNames: artistNames
            );

            // ar jau yra grojaraštyje?
            var already = playlist.PlaylistSongs.Any(ps => ps.SongId == song.Id);
            if (already)
                return (false, "This song is already in the playlist.", null);

            var nextPos = playlist.PlaylistSongs.NextPosition();
            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlist.Id,
                SongId = song.Id,
                Position = nextPos,
                AddedByUserId = request.AddedByUserId,
                AddedAt = DateTime.UtcNow 
            };

            await _playlistRepository.AddPlaylistSongAsync(playlistSong);

            return (true, null, song.Id);
        }
    }
}
