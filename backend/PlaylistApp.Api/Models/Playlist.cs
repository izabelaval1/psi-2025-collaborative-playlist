namespace MyApi.Models
{
    //what data goes through api and how they look in json
    public class Playlist
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<SongDto> Songs { get; set; } = new();
    }

    public class SongDto
    {
        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public string Url { get; set; } = "";
    }
}
