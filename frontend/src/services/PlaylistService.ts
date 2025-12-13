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

  async create(formData: FormData) {
    const res = await api.post(`/api/playlists`, formData);
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
