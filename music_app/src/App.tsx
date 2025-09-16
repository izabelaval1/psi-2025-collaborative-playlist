import { useState } from 'react'
import './App.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';


function App() {
  const playlists = [
    { id: 1, name: 'Chill Vibes', description: 'Relaxing music' },
    { id: 2, name: 'Workout', description: 'Pump up songs' },
  ];

  const displayPlaylists = () => {
    return playlists.map((playlist) => (
      <div key={playlist.id} className="playlist-card">
        <div className="playlist-info">
          <h2>{playlist.name}</h2>
          <p>{playlist.description}</p>
        </div>
        <div className="playlist-icons">
          <FontAwesomeIcon icon={faEdit} className="icon" />
          <FontAwesomeIcon icon={faTrash} className="icon" />
        </div>
      </div>
    ));
  };

  return (
    <div className="playlist-container">{displayPlaylists()}</div>
  )
  
}

export default App
