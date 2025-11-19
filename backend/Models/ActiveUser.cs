namespace MyApi.Models
{
    public class ActivePlaylistUser
{
    public int PlaylistId { get; set; }
    public int UserId { get; set; }
    public DateTime LastActive { get; set; }
}
}