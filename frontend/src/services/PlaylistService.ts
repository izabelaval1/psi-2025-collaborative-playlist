const BASE_URL = "http://localhost:5000/api/playlists";

function authHeaders(): Record<string, string> {
  const token = localStorage.getItem("token");
  return token ? { "Authorization": `Bearer ${token}` } : {};
}

export const PlaylistService = {
  async getById(id: number) {
    const res = await fetch(`${BASE_URL}/${id}`, {
      headers: { ...authHeaders() }
    });
    if (!res.ok) {
      if (res.status === 401 || res.status === 403) throw new Error("Unauthorized. Please log in.");
      throw new Error("Failed to load playlist");
    }
    return res.json();
  },

  async getAll() {
    const res = await fetch(BASE_URL, {
      headers: { ...authHeaders() }
    });
    if (!res.ok) {
      if (res.status === 401 || res.status === 403) throw new Error("Unauthorized. Please log in to see playlists.");
      throw new Error("Failed to load playlists");
    }
    return res.json();
  },

  async create(formData: FormData) {
    const res = await fetch(BASE_URL, {
      method: "POST",
      headers: { ...authHeaders() }, // do NOT set Content-Type for FormData
      body: formData,
    });

    if (!res.ok) throw new Error((await res.text()) || "Failed to create playlist");
    return res.json();
  },

  async update(id: number, data: Partial<any>) {
    const res = await fetch(`${BASE_URL}/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json", ...authHeaders() },
      body: JSON.stringify(data),
    });
    if (!res.ok) throw new Error(await res.text() || "Failed to update playlist");
    return res.json();
  },

  async removeFromPlaylist(playlistId: number, songId: number) {
    const res = await fetch(`${BASE_URL}/${playlistId}/song/${songId}`, {
      method: "DELETE",
      headers: { ...authHeaders() }
    });

    if (!res.ok) throw new Error(await res.text() || "Failed to remove song from playlist");
  },

  async delete(id: number) {
    const res = await fetch(`${BASE_URL}/${id}`, { method: "DELETE", headers: { ...authHeaders() } });
    if (!res.ok) throw new Error(await res.text() || "Failed to delete playlist");
  },

  async addCollaborator(playlistId: number, username: string) {
    const res = await fetch(`${BASE_URL}/${playlistId}/collaborators`, {
      method: "POST",
      headers: { "Content-Type": "application/json", ...authHeaders() },
      body: JSON.stringify({ username }),
    });

    if (!res.ok) {
      try {
        const errorData = await res.json();
        throw new Error(errorData.message || "Failed to add collaborator");
      } catch {
        const errorText = await res.text();
        throw new Error(errorText || "Failed to add collaborator");
      }
    }
  },

  async removeCollaborator(playlistId: number, userId: number) {
    const res = await fetch(`${BASE_URL}/${playlistId}/collaborators/${userId}`, {
      method: "DELETE",
      headers: { ...authHeaders() },
    });

    if (!res.ok) {
      try {
        const errorData = await res.json();
        throw new Error(errorData.message || "Failed to remove collaborator");
      } catch {
        const errorText = await res.text();
        throw new Error(errorText || "Failed to remove collaborator");
      }
    }
  },

  async searchUsers(query: string) {
    const res = await fetch(`http://localhost:5000/api/users/search?q=${encodeURIComponent(query)}`, {
      headers: { ...authHeaders() }
    });
    if (!res.ok) return [];
    return res.json();
  },
};