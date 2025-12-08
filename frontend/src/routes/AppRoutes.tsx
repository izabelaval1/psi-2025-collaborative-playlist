import { Routes, Route, Navigate, useNavigate } from "react-router-dom";

import MainLayout from "../layouts/MainLayout";
import MainPage from "../pages/protected/main/MainPage";
import LandingPage from "../pages/public/landing/landingPage";
import LoginPage from "../pages/public/login/loginPage";
import RegisterPage from "../pages/public/register/registerPage";
import ProtectedRoute from "../components/ProtectedRoute";
import { authService } from "../services/authService";
import Temp from "../pages/protected/temp/Temp";
import Settings from '../pages/public/settings/settingsPage';
import HomePage from "../pages/protected/main/HomePage";
import PlaylistDetailPage from "../pages/protected/main/PlaylistDetailPage";

export default function AppRoutes() {
  const navigate = useNavigate();
  const isAuthenticated = authService.isAuthenticated();
  const user = authService.getUser(); 

  const handleLogout = () => {
    authService.logout();
    navigate("/login"); 
  };

  return (
    <Routes>
      <Route element={<MainLayout isAuthenticated={isAuthenticated} username={user?.username} onLogout={handleLogout} />}>

        {/* Public routes */}
        <Route
        path="/"
        element={isAuthenticated ? <Navigate to="/main" /> : <LandingPage />}
        />

        <Route
          path="/login"
          element={isAuthenticated ? <Navigate to="/main" /> : <LoginPage />}
        />

        <Route
          path="/register"
          element={isAuthenticated ? <Navigate to="/main" /> : <RegisterPage />}
        />

        {/* Protected routes */}
        <Route
          path="/main"
          element={
            <ProtectedRoute>
              <MainPage />
            </ProtectedRoute>
          }
        />
        
        <Route
          path="/temp"
          element={
            <ProtectedRoute>
              <Temp />
            </ProtectedRoute>
          }
        />
        
        <Route
          path="/playlists"
          element={
            <ProtectedRoute>
              <HomePage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/playlist/:id"
          element={
            <ProtectedRoute>
              <PlaylistDetailPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/settings"
          element={
            <ProtectedRoute>
              <Settings />
            </ProtectedRoute>
          }
        />

      </Route>
    </Routes>
  );
}