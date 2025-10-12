using System;
using System.Collections.Generic;

namespace MyApi.Models;

public partial class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    // Playlists the user hosts
    public virtual ICollection<Playlist> HostedPlaylists { get; set; } = new List<Playlist>();

    // Playlists the user collaborates on
    public virtual ICollection<Playlist> CollaboratingPlaylists { get; set; } = new List<Playlist>();
}
