import type { Playlist } from "../types/Playlist";
import type { PlaylistResponseDto } from "../types/PlaylistResponseDto";

const BASE_URL = "http://localhost:5000/api/playlists";

export const PlaylistService = {
  async getById(id: number): Promise<PlaylistResponseDto> {
    const res = await fetch(`${BASE_URL}/${id}`);
    if (!res.ok) throw new Error("Failed to load playlist");
    return res.json();
  },

  async getAll(): Promise<Playlist[]> {
    const res = await fetch(BASE_URL);
    if (!res.ok) throw new Error("Failed to load playlists");
    return res.json();
  },

  async create(formData: FormData): Promise<Playlist> {
    const res = await fetch(BASE_URL, {
      method: "POST",
      body: formData,
    });

    if (!res.ok) throw new Error((await res.text()) || "Failed to create playlist");
    return res.json();
  },

  async update(id: number, data: Partial<Playlist>): Promise<Playlist> {
    const res = await fetch(`${BASE_URL}/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    if (!res.ok) throw new Error(await res.text() || "Failed to update playlist");
    return res.json();
  },

  async removeFromPlaylist(playlistId: number, songId: number): Promise<void> {
    const res = await fetch(`${BASE_URL}/${playlistId}/song/${songId}`, {
      method: "DELETE",
    });

    if (!res.ok) throw new Error(await res.text() || "Failed to remove song from playlist");
  },

  async delete(id: number): Promise<void> {
    const res = await fetch(`${BASE_URL}/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error(await res.text() || "Failed to delete playlist");
  },

  async addCollaborator(playlistId: number, username: string): Promise<void> {
    const token = localStorage.getItem("token");
    
    const res = await fetch(`${BASE_URL}/${playlistId}/collaborators`, {
      method: "POST",
      headers: { 
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify({ username }),
    });

    if (!res.ok) {
      // Parse JSON error response
      try {
        const errorData = await res.json();
        throw new Error(errorData.message || "Failed to add collaborator");
      } catch (parseError) {
        // If JSON parsing fails, use text
        const errorText = await res.text();
        throw new Error(errorText || "Failed to add collaborator");
      }
    }
  },

  async removeCollaborator(playlistId: number, userId: number): Promise<void> {
    const token = localStorage.getItem("token");
    
    const res = await fetch(`${BASE_URL}/${playlistId}/collaborators/${userId}`, {
      method: "DELETE",
      headers: {
        "Authorization": `Bearer ${token}`
      },
    });

    if (!res.ok) {
      try {
        const errorData = await res.json();
        throw new Error(errorData.message || "Failed to remove collaborator");
      } catch (parseError) {
        const errorText = await res.text();
        throw new Error(errorText || "Failed to remove collaborator");
      }
    }
  },

  async searchUsers(query: string): Promise<Array<{ id: number; username: string }>> {
    const res = await fetch(`http://localhost:5000/api/users/search?q=${encodeURIComponent(query)}`);
    if (!res.ok) return [];
    return res.json();
  },
};