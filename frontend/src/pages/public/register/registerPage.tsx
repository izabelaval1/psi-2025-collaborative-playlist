import styles from "./registerPage.module.css"; 
import { UserPlus } from "lucide-react";
import { Link } from "react-router-dom";

const RegisterPage = () => {
  return (
    <div className={styles.loginPage}>
      <div className={styles.loginBox}>
        <h1 className={styles.header}>Create your account</h1>
        <p className={styles.text}>
          Start building collaborative playlists in seconds.
        </p>

        <form>
          <div className={styles.nameRow}>
            <div className={styles.formGroup}>
              <label>First Name</label>
              <input type="text" placeholder="Enter your first name" />
            </div>
            <div className={styles.formGroup}>
              <label>Last Name</label>
              <input type="text" placeholder="Enter your last name" />
            </div>
          </div>

          <div className={styles.formGroup}>
            <label>Email</label>
            <input type="email" placeholder="Enter your email" />
          </div>

          <div className={styles.formGroup}>
            <label>Password</label>
            <input type="password" placeholder="Enter your password" />
          </div>

          <div className={styles.formGroup}>
            <label>Confirm Password</label>
            <input type="password" placeholder="Confirm your password" />
          </div>

          <button className={styles.signInBtn} type="submit">
            <UserPlus className={styles.icon} strokeWidth={2} size={18} />
            Sign Up
          </button>
        </form>

        <p className={styles.signupText}>
          <span className={styles.grey}>Already have an account?{" "}</span>
          <Link to="/login">Log in</Link>
        </p>
      </div>
    </div>
  );
};

export default RegisterPage;
