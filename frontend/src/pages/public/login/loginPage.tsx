import styles from "./LoginPage.module.css";
import { LogIn } from 'lucide-react';
import { Link } from "react-router-dom";

const LoginPage = () => {
  return (
    <div className={styles.loginPage}>
      <div className={styles.loginBox}>
        <h1 className={styles.header}>Welcome Back!</h1>
        <p className={styles.text}>Sign in to continue creating and collaborating on playlists.</p>

        <form>
        <div className={styles.formGroup}>
            <label>Username</label>
            <input type="email" placeholder="Enter your username" />
          </div>

          <div className={styles.formGroup}>
            <label>Email</label>
            <input type="email" placeholder="Enter your email" />
          </div>

          <div className={styles.formGroup}>
            <label>Password</label>
            <input type="password" placeholder="Enter your password" />
          </div>

          <button className={styles.signInBtn} type="submit">
            <LogIn className={styles.icon} strokeWidth={2} size={18}/>
            Sign In</button>
        </form>

        <p className={styles.signupText}>
          <span className={styles.grey}>Don’t have an account?{" "}</span>
          <Link to="/register">Sign up</Link> {" "}
        </p>
      </div>
    </div>
  );
};

export default LoginPage;
