    namespace MyApi.Dtos
{
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
}