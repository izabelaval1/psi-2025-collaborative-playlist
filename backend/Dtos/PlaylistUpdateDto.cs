namespace MyApi.Dtos
{
        public record PlaylistUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<int>? SongIds { get; set; }

        public string? ImageUrl { get; set; }
    }
}