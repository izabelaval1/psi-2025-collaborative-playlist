// handles all API calls for playlists (get, create, update, delete)

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

  async create(data: {
  name: string;
  description?: string;
  hostId: number;
  imageFile?: File;
  }): Promise<Playlist> {
  const formData = new FormData();
  formData.append("name", data.name);
  formData.append("hostId", data.hostId.toString());
  if (data.description) formData.append("description", data.description);
  if (data.imageFile) {
    formData.append("coverImage", data.imageFile); // pavadinimas turi sutapt su backend DTO property
  }

  const res = await fetch(BASE_URL, {
    method: "POST",
    body: formData,           //  be Content-Type, naršyklė pati uždės boundary
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

  if (!res.ok) {
    throw new Error(await res.text() || "Failed to remove song from playlist");
  }
  },

  async delete(id: number): Promise<void> {
    const res = await fetch(`${BASE_URL}/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error(await res.text() || "Failed to delete playlist");
  },
};