// src/hooks/usePlaylists.ts
import { useState, useEffect } from "react";
import type { Playlist } from "../types/Playlist";

export function usePlaylists() {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);

  // Load playlists initially
  useEffect(() => {
    fetch("http://localhost:5000/api/playlists")
      .then(res => res.json())
      .then(data => setPlaylists(data));
  }, []);

  // Add a playlist
  const addPlaylist = (newPlaylist: Playlist) => {
    setPlaylists(prev => [...prev, newPlaylist]);
  };

  // Delete a playlist
  const deletePlaylist = (id: number) => {
    setPlaylists(prev => prev.filter(p => p.id !== id));
  };

  // Update a playlist
  const updatePlaylist = (updated: Playlist) => {
    setPlaylists(prev =>
      prev.map(p => (p.id === updated.id ? updated : p))
    );
  };

  return {
    playlists,
    addPlaylist,
    deletePlaylist,
    updatePlaylist,
  };
}
