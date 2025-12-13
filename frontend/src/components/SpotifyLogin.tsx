import { useEffect, useState, useRef } from "react";
import { useSpotifyPlayer } from "../context/SpotifyPlayerContext";

const API_BASE = "http://localhost:5000/api";

export default function SpotifyLogin() {
  const { setSpotifyToken, spotifyToken } = useSpotifyPlayer();
  const [isLoading, setIsLoading] = useState(false);
  const hasProcessedCallback = useRef(false);

  // Check if we're returning from Spotify OAuth
  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const code = params.get("code");

    if (code && !hasProcessedCallback.current) {
      hasProcessedCallback.current = true;
      handleCallback(code);
    }
  }, []);

  // Exchange code for token
  const handleCallback = async (code: string) => {
    setIsLoading(true);
    try {
      const response = await fetch(`${API_BASE}/SpotifyAuth/callback`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ code }),
      });

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.error || "Failed to get access token");
      }

      const data = await response.json();
      
      // Store tokens
      localStorage.setItem("spotifyToken", data.accessToken);
      if (data.refreshToken) {
        localStorage.setItem("spotifyRefreshToken", data.refreshToken);
      }
      
      setSpotifyToken(data.accessToken);

      // Clean up URL
      window.history.replaceState({}, document.title, window.location.pathname);
    } catch (error) {
      console.error("Authentication failed:", error);
      alert(`Failed to authenticate with Spotify: ${error}`);
      
      // Clear the URL on error so user can try again
      window.history.replaceState({}, document.title, window.location.pathname);
    } finally {
      setIsLoading(false);
    }
  };

  // Initiate Spotify login
  const handleLogin = async () => {
    setIsLoading(true);
    try {
      const response = await fetch(`${API_BASE}/SpotifyAuth/login-url`);
      const data = await response.json();
      
      // Redirect to Spotify authorization
      window.location.href = data.url;
    } catch (error) {
      console.error("Failed to get login URL:", error);
      alert("Failed to start Spotify login");
      setIsLoading(false);
    }
  };

  // Try to restore token from localStorage on mount
  useEffect(() => {
    const savedToken = localStorage.getItem("spotifyToken");
    if (savedToken && !spotifyToken) {
      setSpotifyToken(savedToken);
    }
  }, [spotifyToken, setSpotifyToken]);

  const handleLogout = () => {
    localStorage.removeItem('spotifyToken');
    localStorage.removeItem('spotifyRefreshToken');
    setSpotifyToken(null);
  };

  if (spotifyToken) {
    return (
      <div className="fixed top-4 right-4 flex gap-2 z-50">
        <div className="bg-green-500 text-white px-4 py-2 rounded-lg shadow-lg">
          ✅ Spotify Connected
        </div>
        <button
          onClick={handleLogout}
          className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-lg shadow-lg transition-colors"
        >
          Disconnect
        </button>
      </div>
    );
  }

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-neutral-900 p-8 rounded-xl shadow-2xl text-center max-w-md">
        <div className="text-6xl mb-4">🎵</div>
        <h2 className="text-2xl font-bold text-white mb-4">Connect to Spotify</h2>
        <p className="text-gray-400 mb-6">
          You need to connect your Spotify account to play music.
          <br />
          <span className="text-sm">(Requires Spotify Premium)</span>
        </p>
        
        <button
          onClick={handleLogin}
          disabled={isLoading}
          className="w-full bg-green-500 hover:bg-green-600 disabled:bg-gray-600 text-white font-bold py-3 px-6 rounded-lg transition-colors"
        >
          {isLoading ? "Connecting..." : "Connect with Spotify"}
        </button>

        <p className="text-xs text-gray-500 mt-4">
          This will redirect you to Spotify to authorize the app
        </p>
      </div>
    </div>
  );
}