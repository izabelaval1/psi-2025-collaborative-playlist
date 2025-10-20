namespace MyApi.Dtos
{
        public class PlaylistCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? HostId { get; set; }
    }
}