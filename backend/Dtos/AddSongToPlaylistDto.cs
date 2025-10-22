namespace MyApi.Dtos
{
    public record AddSongToPlaylistDto
    {
        public int PlaylistId { get; set; }
        public string Title { get; set; } = null!;
        public string Artist { get; set; } = null!;
        public string? Album { get; set; }
        public string? Url { get; set; }
    }
}