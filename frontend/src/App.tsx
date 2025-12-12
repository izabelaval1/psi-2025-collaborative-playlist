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
    const token = "BQB0figRO6NR5j3tQICw8aPPqkMHW0yO-5Mj_xncphMp3cJUQu3lzhKPiC-lYzrO9TnqXCK8XPZn1jWHMj1X4mqPBF0zy7igttPFMndJXn1fG2mBfXT0AL95stQflx50HdDsKg96BQ5wTIiIAq4lVTAPukAgi-VmGFX7ho1f1zhlHgwaL-_8xDCcALQmAn6qFkDAv9YJQfYrgRf8TFbipnU0qXybjgTcg8CHhDf_kwR2PZIt";

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
