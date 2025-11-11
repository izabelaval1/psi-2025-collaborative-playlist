using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Dtos;
using MyApi.Utils;
using MyApi.Interfaces;
using MyApi.Data;

namespace MyApi.Services
{
    public class SongService : ISongService
    {
        private readonly PlaylistAppContext _context;

        // dependency injection
        public SongService(PlaylistAppContext context)
        {
            _context = context;
        }

        // Helper metodas - užtikrina, kad daina egzistuoja DB
        private Song EnsureSong(string title, string? album = null, Duration? duration = null, params string[] artistNames)
        {
            // uzkrauna daina ir atlikejus is db
            // patikrina ar tokia daina jau egzistuoja pagal title ir album
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

            // pereina per visus artists
            // jei randa naudojama esama artist
            // jei neranda sukuriamas naujas artist
            foreach (var artistName in artistNames)
            {
                var artist = _context.Artists.FirstOrDefault(a => a.Name == artistName);
                if (artist == null)
                    artist = new Artist { Name = artistName };

                song.Artists.Add(artist);
            }

            // issaugome daina i db
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
        // grazins SongDto sarasagli
        public IEnumerable<SongDto> GetAllSongs()
        {
            // paimu songs is db kartu su susijusiais atlikejais
            // AsNoTracking - nes neplanuoju keisti siu irasu
            var entities = _context.Songs
                .Include(s => s.Artists)
                .AsNoTracking()
                .ToList();

            // converter song -> songDto
            var converter = new GenericConverter<Song, SongDto>();
            var songDtos = converter.ConvertAll(entities, s => new SongDto
            {
                Id = s.Id,
                Title = s.Title,
                Album = s.Album,
                DurationMs = s.DurationSeconds.HasValue
                    ? s.DurationSeconds.Value * 1000
                    : null,
                DurationFormatted = s.DurationSeconds.HasValue
                    ? new Duration(s.DurationSeconds.Value).ToString()
                    : null,
                Artists = s.Artists.Select(a => new ArtistDto
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToList()
            });

            return songDtos;
        }


        // Gauti dainą pagal ID
        public SongDto? GetSongById(int id)
        {
            // paimu songs is db kartu su susijusiais atlikejais
            // AsNoTracking - nes neplanuoju keisti siu irasu
            var song = _context.Songs
                .Include(s => s.Artists)
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == id);

            if (song == null)
                return null;    

            var converter = new GenericConverter<Song, SongDto>();

            var dto = converter.ConvertOne(song, s => new SongDto
            {
                Id = s.Id,
                Title = s.Title,
                Album = s.Album,
                DurationMs = s.DurationSeconds.HasValue
                    ? s.DurationSeconds.Value * 1000
                    : null,
                
                DurationFormatted = s.DurationSeconds.HasValue
                        ? new Duration(s.DurationSeconds.Value).ToString()
                        : null,
                Artists = s.Artists.Select(a => new ArtistDto
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToList()
            });

            return dto;
        }

        // Ištrinti dainą pagal ID
        public (bool Success, string? Error) DeleteSong(int id)
        {
            var song = _context.Songs.Find(id);

            if (song == null)
                return (false, $"Song with ID {id} not found.");

            _context.Songs.Remove(song);
            _context.SaveChanges();

            return (true, null);
        }
    }
}