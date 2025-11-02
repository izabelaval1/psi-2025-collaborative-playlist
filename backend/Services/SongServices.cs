using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Dtos;
using MyApi.Utils;

namespace MyApi.Services
{
    public class SongService
    {
        private readonly PlaylistAppContext _context;

        public SongService(PlaylistAppContext context)
        {
            _context = context;
        }

        // Helper metodas - užtikrina, kad daina egzistuoja DB
        private Song EnsureSong(
            string title,
            string? album = null,
            Duration? duration = null,
            params string[] artistNames)
        {
            var existing = _context.Songs
                .Include(s => s.Artists)
                .FirstOrDefault(s => s.Title == title && s.Album == album);

            if (existing != null)
                return existing;

            var song = new Song
            {
                Title = title,
                Album = album,
                DurationSeconds = duration?.Seconds
            };

            foreach (var artistName in artistNames)
            {
                var artist = _context.Artists.FirstOrDefault(a => a.Name == artistName);
                if (artist == null)
                    artist = new Artist { Name = artistName };

                song.Artists.Add(artist);
            }

            _context.Songs.Add(song);
            _context.SaveChanges();

            return song;
        }

        // Pridėti dainą į playlist
        public (bool Success, string? Error, int? SongId) AddSongToPlaylist(AddSongToPlaylistDto request)
        {
            //  Patikrinti ar playlist egzistuoja
            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefault(p => p.Id == request.PlaylistId);

            if (playlist == null)
                return (false, $"Playlist with ID {request.PlaylistId} not found.", null);

            //  Paruošti atlikėjų sąrašą
            var artistNames = request.ArtistNames
                .Select(a => a.Trim())
                .Where(a => !string.IsNullOrEmpty(a))
                .ToList();

            //  Konvertuoti trukmę
            Duration? duration = null;
            if (request.DurationMs.HasValue)
            {
                duration = Duration.FromMilliseconds(request.DurationMs.Value);
            }

            //  Užtikrinti, kad daina egzistuoja (arba sukurti naują)
            var song = EnsureSong(
                title: request.Title,
                album: request.Album,
                duration: duration,
                artistNames: artistNames.ToArray());

            // Patikrinti ar daina jau yra playlist'e
            var alreadyInPlaylist = playlist.PlaylistSongs.Any(ps => ps.SongId == song.Id);
            if (alreadyInPlaylist)
                return (false, "This song is already in the playlist.", null);

            //  Gauti kitą poziciją
            var nextPosition = playlist.PlaylistSongs.NextPosition();

            //  Pridėti dainą į playlist
            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlist.Id,
                SongId = song.Id,
                Position = nextPosition
            };

            _context.PlaylistSongs.Add(playlistSong);
            _context.SaveChanges();

            return (true, null, song.Id);
        }

        // Gauti visas dainas
        public IEnumerable<SongDto> GetAllSongs()
        {
            // Užkrauti dainas (su atlikėjais) į atmintį
            var entities = _context.Songs
                .Include(s => s.Artists)
                .AsNoTracking()
                .ToList();

            // Rūšiuoti jei reikia
            entities.Sort();

            // Konvertuoti į DTO
            var songs = entities.Select(s => new SongDto
            {
                Id = s.Id,
                Title = s.Title,
                Album = s.Album,
                DurationFormatted = s.DurationSeconds.HasValue
                    ? new Duration(s.DurationSeconds.Value).ToString()
                    : null,
                Artists = s.Artists.Select(a => new ArtistDto
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToList()
            }).ToList();

            return songs;
        }

        // Gauti dainą pagal ID
        public SongDto? GetSongById(int id)
        {
            var song = _context.Songs
                .Include(s => s.Artists)
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SongDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Album = s.Album,
                    DurationFormatted = s.DurationSeconds.HasValue
                        ? new Duration(s.DurationSeconds.Value).ToString()
                        : null,
                    Artists = s.Artists.Select(a => new ArtistDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    }).ToList()
                })
                .FirstOrDefault();

            return song;
        }
    }
}