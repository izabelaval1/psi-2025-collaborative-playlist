import { useState } from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css"; // Tailwind
import "./styles/global.css"; // <-- this must come last
import "./styles/App.css";
import PlaylistList from "./components/PlaylistList";
import PlaylistDisplay from "./components/PlaylistDisplay";
import SongSearch from "./components/SongSearch";
import type { PlaylistResponseDto } from "./types/PlaylistResponseDto.ts";
import PlaylistModal from "./components/PlaylistModal";

function App() {
  const [selectedPlaylist, setSelectedPlaylist] =
    useState<PlaylistResponseDto | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleSongAdded = () => {
    // Refresh the selected playlist after adding a song
    if (selectedPlaylist) {
      fetch(`http://localhost:5000/api/playlists/by-id/${selectedPlaylist.id}`)
        .then((res) => res.json())
        .then((data) => setSelectedPlaylist(data))
        .catch((error) =>
          console.error("Failed to refresh playlist:", error)
        );
    }
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
        <PlaylistList onSelect={setSelectedPlaylist} />
      </aside>

      {/* Divider */}
      <div className="vDivider w-px bg-neutral-800"></div>

      {/* Middle / Right Section */}
      <div className="flex flex-col flex-1 overflow-hidden">
        <SongSearch onSongAdded={handleSongAdded} />

        {/* The playlist display */}
        <div className="flex-1 px-6 pb-6 overflow-hidden mt-4">
          <PlaylistDisplay playlist={selectedPlaylist} />
        </div>
      </div>

      {/* Playlist Modal */}
      {isModalOpen && <PlaylistModal close={() => setIsModalOpen(false)} />}
    </div>
  );
}

export default App;
