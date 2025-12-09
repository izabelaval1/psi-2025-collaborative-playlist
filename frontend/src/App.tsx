import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css";
import "./styles/global.css";
import "./styles/App.css";

import { BrowserRouter as Router } from "react-router-dom";
import AppRoutes from "./routes/AppRoutes";
import { SpotifyPlayerProvider, useSpotifyPlayer } from "./context/SpotifyPlayerContext";
import SpotifyPlayerBar from "./components/SpotifyPlayerBar";
import React, { useEffect } from "react";

function TokenInitializer() {
  const { setSpotifyToken } = useSpotifyPlayer();

  useEffect(() => {
    const token = "longggggasssstokenherefromhttps://developer.spotify.com/documentation/web-playback-sdk/tutorials/getting-startedthatyougetafterlogintooutspotifytowhichcredentialsareinourchatibelieveeeee";

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
