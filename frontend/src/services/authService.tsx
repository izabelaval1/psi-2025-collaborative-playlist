import axios from "axios";

const API_URL = "https://localhost:5000/api/auth";

export const authService = {
  async login(username: string, password: string) {
    const res = await axios.post(`${API_URL}/login`, {
      username,
      password,
    });
    return res.data;
  },

  async register(username: string, password: string) {
    const res = await axios.post(`${API_URL}/register`, {
      username,
      password,
    });
    return res.data;
  },

  logout() {
    localStorage.removeItem("token");
  },

  getToken() {
    return localStorage.getItem("token");
  },
};
