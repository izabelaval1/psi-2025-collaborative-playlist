using MyApi.Dtos;

namespace MyApi.Interfaces
{
    public interface ISongService
    {
        IEnumerable<SongDto> GetAllSongs();
        SongDto? GetSongById(int id);
        (bool Success, string? Error, int? SongId) AddSongToPlaylist(AddSongToPlaylistDto request);
        (bool Success, string? Error) DeleteSong(int id);
    }
}
