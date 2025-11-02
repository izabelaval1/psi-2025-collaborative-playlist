using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // <- needed for [NotMapped]


namespace MyApi.Models;

public partial class Song : IComparable<Song>
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Album { get; set; }
    public int? DurationSeconds { get; set; } // Keep the database column as int

    // Add computed property that uses the struct
    [NotMapped] // <-- tell EF Core to ignore this property
    public Duration? Duration => DurationSeconds.HasValue
        ? new Duration(DurationSeconds.Value)
        : null;

    public ICollection<Artist> Artists { get; set; } = new List<Artist>();
    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

    public int CompareTo(Song? other)
    {
        if (other == null) return 1;
        return string.Compare(Title, other.Title, StringComparison.OrdinalIgnoreCase);
    }
}
