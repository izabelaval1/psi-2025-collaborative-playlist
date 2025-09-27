import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { useState, useEffect } from 'react';

type Playlist = {
  name: string;
  description: string;
  songs: any[];
}; // how playlist looks in frontend

export default function PlaylistList() {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);// array of all playlists from backend
  const [name, setName] = useState("");//setName - updates array
  const [description, setDescription] = useState("");

  // loads data from backend
  useEffect(() => {
    fetch("http://localhost:5000/api/playlist")
      .then(res => res.json())
      .then(data => setPlaylists(data));
  }, []);

  // creates new playlist
  const createPlaylist = () => {
    fetch("http://localhost:5000/api/playlist", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, description, songs: [] })
    })
      .then(res => res.json())
      .then(newPlaylist => setPlaylists([...playlists, newPlaylist]));

    setName("");
    setDescription("");
  };

  // deletes playlist
  const deletePlaylist = (playlistName: string) => {
    fetch(`http://localhost:5000/api/playlist/${playlistName}`, {
      method: "DELETE"
    })
      .then(res => res.json())
      .then(updatedList => setPlaylists(updatedList));
  };

  return (
    <div className="playlist-container">
      <h2>Create new playlist</h2>
      <input
        placeholder="Playlist name"
        value={name}
        onChange={e => setName(e.target.value)}
      />
      <input
        placeholder="Description"
        value={description}
        onChange={e => setDescription(e.target.value)}
      />
      <button onClick={createPlaylist}>Create</button>

      <h2>Playlists:</h2> 
      {playlists.map((playlist, i) => (
        <div key={i} className="playlist-card">
          <div className="playlist-info">
            <h2>{playlist.name}</h2>
            <p>{playlist.description}</p>
          </div>
          <div className="playlist-icons">
            <FontAwesomeIcon icon={faEdit} className="icon" />
            <FontAwesomeIcon
              icon={faTrash}
              className="icon"
              onClick={() => deletePlaylist(playlist.name)} // ← aktyvuota šiukšliadėžė
            />
          </div>
        </div>
      ))}
    </div>
  );
}
