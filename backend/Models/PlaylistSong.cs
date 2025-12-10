namespace MyApi.Models;

public partial class PlaylistSong
{
    public int PlaylistId { get; set; }

    public int SongId { get; set; }

    public int? Position { get; set; }

    public int? AddedByUserId { get; set; }
    public DateTime? AddedAt { get; set; }

    public virtual Playlist Playlist { get; set; } = null!;

    public virtual Song Song { get; set; } = null!;
    
    public virtual User? AddedBy { get; set; }

}