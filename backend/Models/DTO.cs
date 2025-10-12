namespace MyApi.Models
{
    // Response DTOs (what API returns)
    public class PlaylistResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? HostId { get; set; }
        public UserDto? Host { get; set; }
        public List<SongDto> Songs { get; set; } = new();
        public List<UserDto> Collaborators { get; set; } = new();
    }

    public class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Album { get; set; }
        public int? Duration { get; set; }
        public int? Position { get; set; }
        public List<ArtistDto> Artists { get; set; } = new();
    }

    public class ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
    }

    // Request DTOs (what API accepts)
    public class PlaylistCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? HostId { get; set; }
    }

    public class PlaylistUpdateDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<int>? SongIds { get; set; }
    }

    public class PlaylistPatchDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}