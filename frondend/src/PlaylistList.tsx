import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import { useState } from 'react'


export default function PlaylistList() {
  const [playlists, setPlaylists] = useState([
    { id: 1, name: 'Chill Vibes', description: 'Relaxing music' },
    { id: 2, name: 'Workout', description: 'Pump up songs' },
  ]);

  const deletePlaylist = (id: number) => {
    setPlaylists(playlists.filter(playlist => playlist.id !== id));
  }

  return (
    <div className="playlist-container">
      {playlists.map((playlist) => (
        <div key={playlist.id} className="playlist-card">
          <div className="playlist-info">
            <h2>{playlist.name}</h2>
            <p>{playlist.description}</p>
          </div>
          <div className="playlist-icons">
            <FontAwesomeIcon icon={faEdit} className="icon" />
            <FontAwesomeIcon icon={faTrash} className="icon" 
              onClick={() => deletePlaylist(playlist.id)}
            />
          </div>
        </div>
      ))}
    </div>
  );
}
