import { Link } from "react-router-dom";
import { Music2 } from "lucide-react";
import styles from "./Header.module.css";

interface HeaderProps {
  isAuthenticated: boolean;
  username?: string;
  onLogout?: () => void;
}

export default function Header({ isAuthenticated, username, onLogout }: HeaderProps) {
  const publicLinks = [
    { to: "/", label: "Home" },
    { to: "/login", label: "Log in" },
    { to: "/register", label: "Sign Up" },
  ];

  const privateLinks = [
    { to: "/main", label: "Home" },
    { to: "/mysets", label: "My Library" },
    { to: "/settings", label: "Settings" },
  ];

  const links = isAuthenticated ? privateLinks : publicLinks;

  return (
    <header className={styles.header}>
      <div className={styles.left}>
        <Link to="/" className={styles.logo}>
          <Music2 size={28} /> MusicHub
        </Link>
      </div>

      <nav className={styles.navLinks}>
        {links.map((link) => (
          <Link key={link.to} to={link.to}>
            {link.label}
          </Link>
        ))}
      </nav>

      {isAuthenticated && (
        <div className={styles.right}>
          <span className={styles.username}>{username}</span>
          <button className={styles.logoutBtn} onClick={onLogout}>
            Logout
          </button>
        </div>
      )}
    </header>
  );
}
