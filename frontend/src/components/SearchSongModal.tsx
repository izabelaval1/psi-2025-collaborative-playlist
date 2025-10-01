import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import SearchBar from './SearchBar';
import { useState, useEffect } from 'react';

// Define prop types
interface Playlist {
  id: number;
  name: string;
  description: string;
  songs: any[];
}

interface SearchSongModalProps {
  show: boolean;
  onHide: () => void;
}

export default function SearchSongModal(props: SearchSongModalProps) {
  // ← useState must be inside the component
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [selectedPlaylistId, setSelectedPlaylistId] = useState<number | null>(null);

  // ← useEffect must be inside the component
  useEffect(() => {
    if (props.show) {
      fetch('http://localhost:5000/api/playlists')
        .then(res => res.json())
        .then(data => setPlaylists(data));
    }
  }, [props.show]);

  // Function to handle adding a song to a playlist
  const handleAddSong = async (song: any) => {
    if (!selectedPlaylistId) {
      alert('Please select a playlist first!');
      return;
    }

    await fetch(`http://localhost:5000/api/playlists/${selectedPlaylistId}/songs`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(song),
    });

    alert('Song added!');
  };

  return (
    <Modal
      {...props}
      size="lg"
      aria-labelledby="contained-modal-title-vcenter"
      centered
    >
      <Modal.Header closeButton>
        <Modal.Title id="contained-modal-title-vcenter">
          Search a song
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {/* Playlist selector */}
        <select
          value={selectedPlaylistId ?? ''}
          onChange={(e) => setSelectedPlaylistId(Number(e.target.value))}
        >
          <option value="">-- Choose a playlist --</option>
          {playlists.map((p) => (
            <option key={p.id} value={p.id}>{p.name}</option>
          ))}
        </select>

        {/* SearchBar passes songs to handleAddSong */}
        <SearchBar onSongSelect={handleAddSong} />
      </Modal.Body>
      <Modal.Footer>
        <Button onClick={props.onHide}>Close</Button>
      </Modal.Footer>
    </Modal>
  );
}
