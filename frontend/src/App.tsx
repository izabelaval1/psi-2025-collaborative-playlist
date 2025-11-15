import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css";
import "./styles/global.css";
import "./styles/App.css";

import { BrowserRouter as Router } from "react-router-dom";
import AppRoutes from "./routes/AppRoutes"; 

function App() {
  return (
    <Router>
      <AppRoutes />
    </Router>
  );
}

export default App;
