using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // [NotMapped]

namespace MyApi.Models;

public partial class Song : IComparable<Song>
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public string? Album { get; set; }

    public int? DurationSeconds { get; set; } // DB column (nullable int)

    // Computed property for convenience; EF ignores it.
    [NotMapped]
    public Duration? Duration => DurationSeconds.HasValue
        ? new Duration(DurationSeconds.Value)
        : null;

    public ICollection<Artist> Artists { get; set; } = new List<Artist>();

    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

    // IComparable<Song> implementation
    public int CompareTo(Song? other)
    {
        // Any Song > null
        if (other is null) return 1;

        // Primary key: Title (case-sensitive; switch to OrdinalIgnoreCase if desired)
        int byTitle = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if (byTitle != 0) return byTitle;

        //  secondary key: Album (handles nulls: null < non-null)
        int byAlbum = string.Compare(Album, other.Album, StringComparison.Ordinal);
        if (byAlbum != 0) return byAlbum;

        // DurationSeconds (nullable int comparison handles nulls)
        return Nullable.Compare(DurationSeconds, other.DurationSeconds);
    }


}
