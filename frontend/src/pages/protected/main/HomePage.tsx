import RecentPlaylists from "./components/RecentPlaylists";
import SongSearch from "./components/SongSearch";
import { useState, useEffect } from "react";
import type { PlaylistResponseDto } from "../../../types/PlaylistResponseDto";
import type { Playlist } from "../../../types/Playlist";
import { PlaylistService } from "../../../services/PlaylistService";
import "./HomePage.scss";

export default function HomePage() {
  const [selectedPlaylist, setSelectedPlaylist] = useState<PlaylistResponseDto | null>(null);
  const [playlists, setPlaylists] = useState<Playlist[]>([]);

  useEffect(() => {
    const loadPlaylists = async () => {
      try {
        const all = await PlaylistService.getAll();
        setPlaylists(all);
      } catch (err) {
        console.error("Failed to load playlists:", err);
      }
    };

    loadPlaylists();
  }, []);

  const handleSongListChanged = async () => {
    if (!selectedPlaylist) return;

    try {
      const updated = await PlaylistService.getById(selectedPlaylist.id);
      setSelectedPlaylist(updated);
    } catch (err) {
      console.error("Failed to refresh playlist:", err);
    }
  };

  const handlePlaylistClick = async (playlist: Playlist) => {
    try {
      const full = await PlaylistService.getById(playlist.id);
      setSelectedPlaylist(full);
    } catch (err) {
      console.error("Failed to load playlist:", err);
    }
  };

  return (
    <div className="home-page">
      <SongSearch onSongAdded={handleSongListChanged} playlists={playlists} />
      <RecentPlaylists playlists={playlists} onPlaylistClick={handlePlaylistClick} />
    </div>
  );
}