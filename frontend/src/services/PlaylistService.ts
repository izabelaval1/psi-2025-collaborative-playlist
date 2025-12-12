import api from "./api";

export const PlaylistService = {
  async getById(id: number) {
    const res = await api.get(`/api/playlists/${id}`);
    return res.data;
  },

  async getAll() {
    const res = await api.get(`/api/playlists`);
    return res.data;
  },

   async create(input: { name: string; description?: string; hostId: number; imageFile?: File }) {
    const fd = new FormData();
    fd.append("name", input.name);
    if (input.description) fd.append("description", input.description);
    fd.append("hostId", String(input.hostId));
    if (input.imageFile) fd.append("imageFile", input.imageFile);

    const res = await api.post("/api/playlists", fd);
    return res.data;
  },

  async update(id: number, data: Partial<any>) {
    const res = await api.patch(`/api/playlists/${id}`, data);
    return res.data;
  },

  async removeFromPlaylist(playlistId: number, songId: number) {
    await api.delete(`/api/playlists/${playlistId}/song/${songId}`);
  },

  async delete(id: number) {
    await api.delete(`/api/playlists/${id}`);
  },

  async addCollaborator(playlistId: number, username: string) {
    await api.post(`/api/playlists/${playlistId}/collaborators`, { username });
  },

  async removeCollaborator(playlistId: number, userId: number) {
    await api.delete(`/api/playlists/${playlistId}/collaborators/${userId}`);
  },

  async searchUsers(query: string) {
    const res = await api.get(`/api/users/search`, { params: { q: query } });
    return res.data ?? [];
  },
};
