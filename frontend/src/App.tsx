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
    const token = "BQB5skRCCvUoEnzHLb_cVbIUPImc67LWnYGnxWhb2xRMMeeRg57yMMTILwHO1e-m79NFgtXrvChEnUHunkaWJ9HcbLmmBFRWgyDuFK-3ANkkplwv9iGuISK7EXKlw538UJY4Luid82bmaa4i8eUY2aucOL2g0Pga8em4SBAz9MOMCcqb65ApUQqUy_evz1OixG98o58xOybzK4Is5pGEelye_8Y1_FhceGmwQQN7b6o2x5_TPbje";

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
