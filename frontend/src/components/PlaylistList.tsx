import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { useState, useEffect } from 'react';

type Playlist = {
  id: number;
  name: string;
  description: string;
  songs: any[];
};//what playlist will look like in frontend, matches backend playlist model

export default function PlaylistList() {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  // Editing state
  const [editingId, setEditingId] = useState<number | null>(null);//tracks which playlist is being edited
  const [editName, setEditName] = useState("");
  const [editDescription, setEditDescription] = useState("");//temporary values

  // load data
  useEffect(() => {
    fetch("http://localhost:5000/api/playlists")
      .then(res => res.json())
      .then(data => setPlaylists(data));
  }, []);

  // create playlist
  const createPlaylist = () => {
    fetch("http://localhost:5000/api/playlists", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, description, songs: [] })
    })
      .then(res => res.json())
      .then(newPlaylist => setPlaylists([...playlists, newPlaylist]));

    setName("");
    setDescription("");
  };

  // delete playlist
  const deletePlaylist = (playlistId: number) => {
    fetch(`http://localhost:5000/api/playlists/${playlistId}`, {
      method: "DELETE"
    }).then(() => {
      setPlaylists(playlists.filter(p => p.id !== playlistId));
    });
  };

  //saves the changes
  //calls PUT method, backend saves changes in json, react updates the specific playlist
  const saveEdit = (playlistId: number) => {
  fetch(`http://localhost:5000/api/playlists/by-id/${playlistId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      id: playlistId,
      name: editName,
      description: editDescription,
      songs: playlists.find(p => p.id === playlistId)?.songs || []
    })
  })
    .then(res => res.json())
    .then(updatedPlaylist => {
      setPlaylists(playlists.map(p => (p.id === playlistId ? updatedPlaylist : p)));
      setEditingId(null); // exit edit mode
    });
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
      {playlists.map((playlist) => (
        <div key={playlist.id} className="playlist-card">
          {editingId === playlist.id ? (
            <>
              <input
                value={editName}
                onChange={e => setEditName(e.target.value)}
              />
              <input
                value={editDescription}
                onChange={e => setEditDescription(e.target.value)}
              />
              <button onClick={() => saveEdit(playlist.id)}>Save</button>
              <button onClick={() => setEditingId(null)}>Cancel</button>
            </>
          ) : (
            <>
              <div className="playlist-info">
                <h2>{playlist.name}</h2>
                <p>{playlist.description}</p>
              </div>
              <div className="playlist-icons">
                <FontAwesomeIcon
                  icon={faEdit}
                  className="icon"
                  onClick={() => {
                    setEditingId(playlist.id);
                    setEditName(playlist.name);
                    setEditDescription(playlist.description);
                  }}
                />
                <FontAwesomeIcon
                  icon={faTrash}
                  className="icon"
                  onClick={() => deletePlaylist(playlist.id)}
                />
              </div>
            </>
          )}
        </div>
      ))}
    </div>
  );
}
