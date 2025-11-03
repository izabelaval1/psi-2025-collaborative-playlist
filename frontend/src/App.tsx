import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css";
import "./styles/global.css";
import "./styles/App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import MainPage from "./pages/protected/main/MainPage";
import LandingPage from "./pages/public/landing/landingPage";
import LoginPage from "./pages/public/login/loginPage";
import RegisterPage from "./pages/public/register/registerPage";
function App() {
  return (
    <Router>
      <Routes>
        {/* public routes */}
        <Route path="/" element={<LandingPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        {/* potected (not yet:D) routes  */}
        <Route path="/main" element={<MainPage />} />
        
      </Routes>
    </Router>
  );
}
export default App;