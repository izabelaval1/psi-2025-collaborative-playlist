namespace MyApi.Dtos
{
    public record ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}