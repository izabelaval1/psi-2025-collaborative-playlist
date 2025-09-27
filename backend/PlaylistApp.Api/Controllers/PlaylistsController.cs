using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using System.Text.Json;

namespace MyApi.Controllers
{
    [ApiController] // web api controller 
    [Route("api/[controller]")] // url will be api/playlist (according to the name of the controller)
    public class PlaylistsController : ControllerBase
    {
      
        private static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "playlists1.json"); //// json file location

        // Helper function reads all playlists from the JSON file
        private List<Playlist> LoadPlaylists()
        {
            try
            {
                if (!System.IO.File.Exists(FilePath)) // if non-existing returns empty list
                    return new List<Playlist>();

                var json = System.IO.File.ReadAllText(FilePath); //if existing - reads the file

                var playlists = JsonSerializer.Deserialize<List<Playlist>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true //  mhm allow lowercase JSON keys
                    });
                return playlists ?? new List<Playlist>(); // if null, return empty list
            }
            catch
            {
                return new List<Playlist>(); // on error → return empty list
            }
        }

        // helper to save playlists back to the JSON file
        private void SavePlaylists(List<Playlist> playlists)
        {
            var json = JsonSerializer.Serialize(playlists, new JsonSerializerOptions { WriteIndented = true }); // Serialize list → JSON
            Directory.CreateDirectory("Data"); // Ensure the folder exists
            System.IO.File.WriteAllText(FilePath, json); // updates list overwrites
        }

        // GET /api/playlists -> return all playlists
        [HttpGet]
        public IActionResult GetPlaylists()
        {
            return Ok(LoadPlaylists());
        }

        // GET /api/playlists/{id} -> return playlist by ID
        [HttpGet("{id:int}")]
        public IActionResult GetPlaylistById(int id)
        {
            var playlist = LoadPlaylists().FirstOrDefault(p => p.Id == id); // Find playlist by ID
            if (playlist == null)
                return NotFound(); // if not found → 404

            return Ok(playlist); // if found → return playlist
        }

        // POST /api/playlists -> create a new playlist
        [HttpPost]
        public IActionResult CreatePlaylist([FromBody] Playlist newPlaylist)
        {
            var playlists = LoadPlaylists(); // Load current playlists

            // auto new ID (+1 from last one or 1 if list empty)
            int newId;

            if (playlists.Count > 0)
            {
                var lastPlaylist = playlists[playlists.Count - 1];
                newId = lastPlaylist.Id + 1;
            }
            else
            {
                newId = 1;
            }

            newPlaylist.Id = newId;

            playlists.Add(newPlaylist); // Add to list
            SavePlaylists(playlists); // Save back to JSON

            return CreatedAtAction(nameof(GetPlaylistById), new { id = newPlaylist.Id }, newPlaylist); // Return 201 Created
        }

        // PUT /api/playlists/{id} -> full update (replace name & description)
        [HttpPut("{id:int}")]
        public IActionResult UpdatePlaylist(int id, [FromBody] Playlist updatedPlaylist)
        {
            var playlists = LoadPlaylists();
            var playlist = playlists.FirstOrDefault(p => p.Id == id); // find playlist by ID
            if (playlist == null)
                return NotFound(); // If not found → 404

            // Overwrite fields
            playlist.Name = updatedPlaylist.Name;
            playlist.Description = updatedPlaylist.Description;

            SavePlaylists(playlists); 
            return Ok(playlist); // return updated playlist
        }

        // PATCH /api/playlists/{id} -> partial update (only update provided fields)
        [HttpPatch("{id:int}")]
        public IActionResult EditPlaylist(int id, [FromBody] Playlist editedPlaylist)
        {
            var playlists = LoadPlaylists();
            var playlist = playlists.FirstOrDefault(p => p.Id == id);
            if (playlist == null)
                return NotFound();

            // Only update provided fields
            if (!string.IsNullOrEmpty(editedPlaylist.Name))
                playlist.Name = editedPlaylist.Name;

            if (!string.IsNullOrEmpty(editedPlaylist.Description))
                playlist.Description = editedPlaylist.Description;

            SavePlaylists(playlists);
            return Ok(playlist);
        }

        // DELETE /api/playlists/{id} -> delete playlist by ID
        [HttpDelete("{id:int}")]
        public IActionResult DeletePlaylistById(int id)
        {
            var playlists = LoadPlaylists();
            var playlist = playlists.FirstOrDefault(p => p.Id == id);
            if (playlist == null)
                return NotFound();

            playlists.Remove(playlist); 
            SavePlaylists(playlists); 

            return NoContent(); // 204
        }

        // DELETE /api/playlists/by-name/{name} -> delete playlist by name
        [HttpDelete("by-name/{name}")]
        public IActionResult DeletePlaylistByName(string name)
        {
            var playlists = LoadPlaylists();
            var playlist = playlists.FirstOrDefault(p => p.Name == name);
            if (playlist == null)
                return NotFound();

            playlists.Remove(playlist);
            SavePlaylists(playlists);

            return NoContent(); // return 204 
        }
    }
}
