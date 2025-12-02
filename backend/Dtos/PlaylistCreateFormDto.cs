namespace MyApi.Dtos
{
    public record PlaylistCreateFormDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int HostId { get; set; }
        public IFormFile? CoverImage { get; set; }
    }
}
