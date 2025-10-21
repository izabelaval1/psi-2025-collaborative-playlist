import { useState, useRef } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css";
import "./styles/global.css";
import "./styles/App.css";

import PlaylistList, { type PlaylistListHandle } from "./components/PlaylistList";
import PlaylistDisplay from "./components/PlaylistDisplay";
import SongSearch from "./components/SongSearch";
import PlaylistModal from "./components/PlaylistModal";
import type { PlaylistResponseDto } from "./types/PlaylistResponseDto";
import type { Playlist } from "./types/Playlist";

function App() {
  const [selectedPlaylist, setSelectedPlaylist] =
    useState<PlaylistResponseDto | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [playlists, setPlaylists] = useState<Playlist[]>([]);

  const playlistListRef = useRef<PlaylistListHandle>(null);

  const handleSongAdded = () => {
    if (selectedPlaylist) {
      fetch(`http://localhost:5000/api/playlists/${selectedPlaylist.id}`)
        .then((res) => res.json())
        .then((data) => setSelectedPlaylist(data))
        .catch((error) =>
          console.error("Failed to refresh playlist:", error)
        );
    }
  };

  const handlePlaylistsLoaded = (loadedPlaylists: Playlist[]) => {
    setPlaylists(loadedPlaylists);
  };

  return (
    <div className="main-page flex h-screen bg-neutral-950 text-white">
      {/* Left sidebar */}
      <aside className="left-side w-48 p-4 border-r border-neutral-800">
        <button
          className="mb-4 w-full bg-green-500 hover:bg-green-600 text-white py-2 rounded-lg font-semibold"
          onClick={() => setIsModalOpen(true)}
        >
          + New Playlist
        </button>

        <PlaylistList 
          ref={playlistListRef} 
          onSelect={setSelectedPlaylist}
          onPlaylistsLoaded={handlePlaylistsLoaded}
        />
      </aside>

      {/* Divider */}
      <div className="vDivider w-px bg-neutral-800"></div>

      {/* Middle / Right Section */}
      <div className="flex flex-col flex-1 overflow-hidden">
        <SongSearch 
          onSongAdded={handleSongAdded}
          playlists={playlists}
        />

        {/* The playlist display */}
        <div className="flex-1 px-6 pb-6 overflow-hidden mt-4">
          <PlaylistDisplay playlist={selectedPlaylist} />
        </div>
      </div>

      {/* Playlist Modal */}
      {isModalOpen && (
        <PlaylistModal
          close={() => setIsModalOpen(false)}
          onPlaylistCreated={async () => {
            setIsModalOpen(false);
            await playlistListRef.current?.refresh();
          }}
        />
      )}
    </div>
  );
}

export default App;