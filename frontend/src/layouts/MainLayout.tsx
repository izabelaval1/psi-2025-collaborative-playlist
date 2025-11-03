import { Outlet } from "react-router-dom";
import Header from "../components/header/Header";

interface MainLayoutProps {
  isAuthenticated: boolean;
  username?: string;
  onLogout?: () => void;
}

export default function MainLayout({ isAuthenticated, username, onLogout }: MainLayoutProps) {
  return (
    <div>
      <Header isAuthenticated={isAuthenticated} username={username} onLogout={onLogout} />
      
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
