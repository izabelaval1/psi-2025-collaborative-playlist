import "bootstrap/dist/css/bootstrap.min.css";
import "./index.css";
import "./styles/global.css";
import "./styles/App.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import MainPage from "./pages/protected/main/MainPage";
import LandingPage from "./pages/public/landing/LandingPage";
import LoginPage from "./pages/public/login/loginPage";
import RegisterPage from "./pages/public/register/registerPage";

function App() {
  const isAuthenticated = false; // bus pakeista auth logic
  const username = "userTemporary"; // bus pakeista i tikra

  const handleLogout = () => {
    console.log("Logout clicked");
    //logout logic
  };

  return (
    <Router>
      <Routes>
        {/* Wrap routes that share header inside MainLayout */}
        <Route
          element={
            <MainLayout
              isAuthenticated={isAuthenticated}
              username={username}
              onLogout={handleLogout}
            />
          }
        >
          {/* public routes */}
          <Route path="/" element={<LandingPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* protected routes */}
          <Route path="/main" element={<MainPage />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;