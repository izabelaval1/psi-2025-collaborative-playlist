//komentaras nes su gitu kazkas blogai
import type { Playlist } from "../types/Playlist";

const BASE_URL = "http://localhost:5000/api/playlists";

export const PlaylistService = {
  async getAll(): Promise<Playlist[]> {
    const res = await fetch(BASE_URL);
    if (!res.ok) throw new Error("Failed to load playlists");
    return res.json();
  },

  async delete(id: number): Promise<void> {
    const res = await fetch(`${BASE_URL}/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error(await res.text() || "Delete failed");
  },

  async update(id: number, data: Partial<Playlist>): Promise<Playlist> {
    const res = await fetch(`${BASE_URL}/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    if (!res.ok) throw new Error(await res.text() || "Failed to save edit");
    return res.json();
  },

  async create(data: { name: string; description?: string; hostId: number }): Promise<Playlist> {
    const res = await fetch(BASE_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    if (!res.ok) throw new Error(await res.text() || "Failed to create playlist");
    return res.json();
  },

    async removeFromPlaylist(playlistId: number, songId: number): Promise<void> {
  const res = await fetch(`${BASE_URL}/${playlistId}/song/${songId}`, { method: "DELETE" });
  if (!res.ok) throw new Error(await res.text() || "Failed to remove song from playlist");
}
};
