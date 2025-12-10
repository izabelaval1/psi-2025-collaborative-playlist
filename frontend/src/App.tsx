import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css";
import "./styles/global.scss";
import "./styles/App.css";

import { BrowserRouter as Router } from "react-router-dom";
import AppRoutes from "./routes/AppRoutes";
import { SpotifyPlayerProvider, useSpotifyPlayer } from "./context/SpotifyPlayerContext";
import SpotifyPlayerBar from "./components/SpotifyPlayerBar";
import React, { useEffect } from "react";

function TokenInitializer() {
  const { setSpotifyToken } = useSpotifyPlayer();

  useEffect(() => {
    const token = "BQCqJnAKvdxBvtHh5oQIxxbpmCzXyntizWV_xpfJKnUYe5B5S3dhh6euMhs5Mpj8k4N0bcpc4GNVjxyZQuVwpQJgRUzNLTLyZfmy6FSmAceqPBvqySy_7-mmv9IsGcNdUwc3bQ9MyRqVRWnNxVQGVidsmFoMOz2ZErvBM6bT1WiejZP0dyMymTnB2TJSUvsU3mBxNfNv_VjaHpMhYlwxYFD_lc_9x2LmLw9gmLWE__3tsQDjwct8";

    if (!token) {
      console.error("NO SPOTIFY TOKEN PROVIDED — The player will NOT work.");
      return;
    }

    setSpotifyToken(token);
  }, []);

  return null;
}

function App() {
  return (
    <SpotifyPlayerProvider>
      <TokenInitializer />
      <Router>
        <AppRoutes />
      </Router>
      <SpotifyPlayerBar />
    </SpotifyPlayerProvider>
  );
}

export default App;
