namespace MyApi.Models;

public partial class Playlist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? HostId { get; set; } // the user who owns the playlist

    // Navigation to the hosting user
    public virtual User? Host { get; set; }

    public string? ImageUrl { get; set; }

    


    // Many-to-many relationship: collaborators
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    // Playlist-Song junction table
    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}
