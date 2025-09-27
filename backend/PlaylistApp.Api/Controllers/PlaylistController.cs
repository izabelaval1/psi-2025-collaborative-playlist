using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using System.Text.Json;

namespace MyApi.Controllers
{
    [ApiController] // web api controller 
    [Route("api/[controller]")] // url will be api/playlist (according to the name of the controller)
    public class PlaylistController : ControllerBase
    {
        private const string FilePath = "Data/playlists.json"; // json file location

        [HttpGet]
        public IActionResult GetPlaylists()
        {
            if (!System.IO.File.Exists(FilePath)) // if non-existing returns empty list
                return Ok(new List<Playlist>());

            var json = System.IO.File.ReadAllText(FilePath);// if existing - reads the file
            var playlists = JsonSerializer.Deserialize<List<Playlist>>(json) ?? new List<Playlist>(); //json -> list
            return Ok(playlists);// api/playlist -> all playlists in json
        }

        [HttpPost]
        public IActionResult CreatePlaylist([FromBody] Playlist playlist)
        {
            List<Playlist> playlists; //takes playlist object from FormBody

            if (System.IO.File.Exists(FilePath))// reads all current playlists ??
            {
                var json = System.IO.File.ReadAllText(FilePath);
                playlists = JsonSerializer.Deserialize<List<Playlist>>(json) ?? new List<Playlist>();
            }
            else // creates new playlist
            {
                playlists = new List<Playlist>();
            }

            playlists.Add(playlist); // adds playlist to list 

            var updatedJson = JsonSerializer.Serialize(playlists, new JsonSerializerOptions { WriteIndented = true }); //serialize -> back to json
            Directory.CreateDirectory("Data");
            System.IO.File.WriteAllText(FilePath, updatedJson); // updates list

            return Ok(playlist); // returns new playlist
        }

        [HttpDelete("{name}")]
        public IActionResult DeletePlaylist(string name)
        {
            if (!System.IO.File.Exists(FilePath)) // -> 404 not found
                return NotFound();

            var json = System.IO.File.ReadAllText(FilePath); // reads everything
            var playlists = JsonSerializer.Deserialize<List<Playlist>>(json) ?? new List<Playlist>();

            var playlist = playlists.FirstOrDefault(p => p.Name == name);  // finds playlist by name 
            if (playlist == null)
                return NotFound(); // -> 404 not found

            playlists.Remove(playlist);

            var updatedJson = JsonSerializer.Serialize(playlists, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(FilePath, updatedJson);

            return Ok(playlists);
        }
    }
}
