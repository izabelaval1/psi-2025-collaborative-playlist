import RecentPlaylists from "./components/RecentPlaylists";
import SongSearch from "./components/SongSearch";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import type { Playlist } from "../../../types/Playlist";
import { PlaylistService } from "../../../services/PlaylistService";
import "./HomePage.scss";
import CreatePlaylistForm from "./components/CreatePlaylistForm";
import Modal from "../../../components/Modal";

export default function HomePage() {
  const navigate = useNavigate();
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);

  const loadPlaylists = async () => {
    try {
      const all = await PlaylistService.getAll();
      setPlaylists(all);
    } catch (err) {
      console.error("Failed to load playlists:", err);
    }
  };

  useEffect(() => {
    loadPlaylists();
  }, []);

  const handleSongListChanged = async () => {
    // Reload playlists when a song is added
    await loadPlaylists();
  };

  const handlePlaylistClick = (playlist: Playlist) => {
    // Navigate to playlist detail page
    navigate(`/playlist/${playlist.id}`);
  };

  const handlePlaylistCreated = async () => {
    setIsModalOpen(false);
    await loadPlaylists();
  };

  // Called when a playlist is updated inside a card
  const handlePlaylistUpdated = async (updated: Playlist) => {
    // Simple approach: reload list to get latest data
    await loadPlaylists();
  };

  // Called when a playlist is deleted inside a card
  const handlePlaylistDeleted = async (id: number) => {
    await loadPlaylists();
  };

  return (
    <div className="home-page">
      <SongSearch onSongAdded={handleSongListChanged} playlists={playlists} />
      <RecentPlaylists
        playlists={playlists}
        onPlaylistClick={handlePlaylistClick}
        onCreateClick={() => setIsModalOpen(true)}
        onPlaylistUpdated={handlePlaylistUpdated}
        onPlaylistDeleted={handlePlaylistDeleted}
      />

      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="New collaborative playlist"
      >
        <CreatePlaylistForm
          onPlaylistCreated={handlePlaylistCreated}
          onCancel={() => setIsModalOpen(false)}
          hostId={1} // TODO: Replace with actual user ID from auth context
        />
      </Modal>
    </div>
  );
}