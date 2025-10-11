namespace MyApi.Models
{
    ////what data goes through api and how they look in json
    public class Playlist//class
    {

        public int Id { get; set; }//properties, get is used for displaying value, set for setting value
        public string Name { get; set; } = ""; // "" - kad nebutu priskirtas null (null possibly veiktu tik su string?)
        public string Description { get; set; } = "";
        public List<SongDto> Songs { get; set; } = new();
    }

    public class SongDto //dto - data transfer object - paprastas duomenu konteineris (be logikos) pernest duomenims per api i json
    {
        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public string Url { get; set; } = "";
    }
}
