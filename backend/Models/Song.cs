using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Models;

public partial class Song : IComparable<Song>
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;
    public string? Album { get; set; }
    public int? DurationSeconds { get; set; }

    [Column("spotify_id")]
    public string SpotifyId { get; set; } = default!;

    [Column("spotify_uri")]
    public string SpotifyUri { get; set; } = default!;

    [NotMapped]
    public Duration? Duration => DurationSeconds.HasValue
        ? new Duration(DurationSeconds.Value)
        : null;

    public ICollection<Artist> Artists { get; set; } = new List<Artist>();
    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

    public int CompareTo(Song? other)
    {
        if (other is null) return 1;

        int byTitle = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if (byTitle != 0) return byTitle;

        int byAlbum = string.Compare(Album, other.Album, StringComparison.Ordinal);
        if (byAlbum != 0) return byAlbum;

        return Nullable.Compare(DurationSeconds, other.DurationSeconds);
    }
}
