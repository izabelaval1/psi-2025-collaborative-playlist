using System;
using System.Collections.Generic;

namespace MyApi.Models;

public partial class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Album { get; set; }
    public int? Duration { get; set; }

    public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();
    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}