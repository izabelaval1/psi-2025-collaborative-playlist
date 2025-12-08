namespace MyApi.Dtos
{
    public class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Album { get; set; }
        public int? Duration { get; set; }

        public string? DurationFormatted { get; set; }

        public int? Position { get; set; }
        public List<ArtistDto> Artists { get; set; } = new();

        public UserDto? AddedBy { get; set; }
        public DateTime? AddedAt { get; set; }

        
    }
}
