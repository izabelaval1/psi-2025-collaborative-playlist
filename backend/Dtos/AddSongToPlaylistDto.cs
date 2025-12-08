namespace MyApi.Dtos
{
    public record AddSongToPlaylistDto
    {
        public int PlaylistId { get; set; }
        public string Title { get; set; } = null!;
        public List<string> ArtistNames { get; set; } = new(); // replaces single Artist
        public string? Album { get; set; }
        public string? Url { get; set; }

        // Needed for Duration
        public int? DurationMs { get; set; }
         public int? AddedByUserId { get; set; }
    }
}
