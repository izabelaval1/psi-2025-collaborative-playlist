namespace MyApi.Dtos
{

        public class PlaylistUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<int>? SongIds { get; set; }
    }
}