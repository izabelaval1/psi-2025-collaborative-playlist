using System.Text.Json.Serialization;

namespace MyApi.Dtos
{
    public class SpotifyTrackDetails
    {
        [JsonPropertyName("id")]
        public string SpotifyId { get; set; } = null!;

        [JsonPropertyName("uri")]
        public string SpotifyUri { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("album")]
        public SpotifyAlbumDetails AlbumInfo { get; set; } = null!;

        [JsonPropertyName("duration_ms")]
        public int? DurationMs { get; set; }

        [JsonPropertyName("artists")]
        public List<SpotifyArtistDetails> Artists { get; set; } = new();

        public List<string> ArtistNames => Artists.Select(a => a.Name).ToList();
    }

    public class SpotifyAlbumDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }

    public class SpotifyArtistDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}