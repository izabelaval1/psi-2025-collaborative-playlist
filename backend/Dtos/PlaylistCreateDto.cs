namespace MyApi.Dtos
{
        public record PlaylistCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int HostId { get; set; }
    }
}