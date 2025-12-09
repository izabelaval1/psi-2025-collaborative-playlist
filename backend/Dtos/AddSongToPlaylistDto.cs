namespace MyApi.Dtos
{
    public record AddSongToPlaylistDto
    {
        public int PlaylistId { get; set; }
        public string Title { get; set; } = null!;
        public List<string> ArtistNames { get; set; } = new();
        public string? Album { get; set; }
        public string? Url { get; set; }
        public int? DurationMs { get; set; }
        public string SpotifyId { get; set; } = null!;
        public string SpotifyUri { get; set; } = null!;
    }
}