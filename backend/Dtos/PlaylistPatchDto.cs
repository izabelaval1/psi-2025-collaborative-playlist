namespace MyApi.Dtos
{
    public record PlaylistPatchDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}