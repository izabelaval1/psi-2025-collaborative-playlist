import axios from "axios";

const API_URL = "http://localhost:5000/api/auth"; 

export const authService = {
  async login(username: string, password: string) {
    const res = await axios.post(`${API_URL}/login`, {
      username,
      password,
    });
    
    // Išsaugoti token
    if (res.data.token) {
      localStorage.setItem("token", res.data.token);
      if (res.data.user) {
        localStorage.setItem("user", JSON.stringify(res.data.user));
      }
    }
    
    return res.data;
  },

  async register(username: string, password: string, confirmPassword: string) {
    const res = await axios.post(`${API_URL}/register`, {
      username,
      password,
      confirmPassword,
    });
    
    // Išsaugoti token po registracijos
    if (res.data.token) {
      localStorage.setItem("token", res.data.token);
      if (res.data.user) {
        localStorage.setItem("user", JSON.stringify(res.data.user));
      }
    }
    
    return res.data;
  },

  logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  },

  getToken() {
    return localStorage.getItem("token");
  },
  
  getUser() {
    const user = localStorage.getItem("user");
    return user ? JSON.parse(user) : null;
  },
  
  isAuthenticated() {
    return !!this.getToken();
  },
};