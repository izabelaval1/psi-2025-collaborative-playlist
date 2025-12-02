import { authService } from "./authService";

const API_BASE = "http://localhost:5000/api";

export const UserService = {
  async updateProfileImage(userId: number, imageFile: File) {
    const formData = new FormData();
    formData.append('imageFile', imageFile);

    const res = await fetch(`${API_BASE}/users/${userId}/profile-image`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${authService.getToken()}`
      },
      body: formData
    });

    if (!res.ok) throw new Error('Failed to update profile image');
    return res.json();
  },

  async getCurrentUser() {
    const res = await fetch(`${API_BASE}/users/me`, { // arba /users/{id}
      headers: {
        'Authorization': `Bearer ${authService.getToken()}`
      }
    });
    if (!res.ok) throw new Error('Failed to get user');
    return res.json();
  }
};