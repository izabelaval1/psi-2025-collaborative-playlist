//uses the playlistService to fetch data and updates React state automatically.
//Manages React state and UI behavior.

import { useState, useEffect } from "react";
import type { Playlist } from "../types/Playlist";
import { PlaylistService } from "../services/PlaylistService";

export function usePlaylists() {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch playlists initially
  useEffect(() => {
    const load = async () => {
      try {
        const data = await PlaylistService.getAll();
        setPlaylists(data);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  // Add playlist
  const addPlaylist = async (data: { name: string; description?: string; hostId: number }) => {
    const created = await PlaylistService.create(data);
    setPlaylists(prev => [...prev, created]);
  };

  // Delete playlist
  const deletePlaylist = async (id: number) => {
    await playlistService.delete(id);
    setPlaylists(prev => prev.filter(p => p.id !== id));
  };

  // Update playlist
  const updatePlaylist = async (id: number, updates: Partial<Playlist>) => {
    const updated = await PlaylistService.update(id, updates);
    setPlaylists(prev => prev.map(p => (p.id === id ? updated : p)));
  };

  return {
    playlists,
    loading,
    error,
    addPlaylist,
    deletePlaylist,
    updatePlaylist,
  };
}
