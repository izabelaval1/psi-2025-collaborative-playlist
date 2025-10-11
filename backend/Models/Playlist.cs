namespace MyApi.Models
{
    ////what data goes through api and how they look in json
    public class Playlist//class
    {
        public int Id { get; set; }//properties, get is used for displaying value, set for setting value
        public string Name { get; set; } = ""; // "" - kad nebutu priskirtas null (null possibly veiktu tik su string?)
        public string Description { get; set; } = "";
        public int? HostId { get; set; }
    }
       

}