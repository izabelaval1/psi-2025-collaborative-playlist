namespace MyApi.Models;
public class Song : IComparable<Song>
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Album { get; set; }
    public int? Duration { get; set; }

    public ICollection<Artist> Artists { get; set; } = new List<Artist>();
    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

    public int CompareTo(Song? other)
    {
        // any Song > null
        if (other is null) return 1;

        int byTitle = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if (byTitle != 0) return byTitle; // primary key

        int byAlbum = string.Compare(Album, other.Album, StringComparison.Ordinal);
        if (byAlbum != 0) return byAlbum; //secondary key

        return Nullable.Compare(Duration, other.Duration); //handles nulls
    }
}
