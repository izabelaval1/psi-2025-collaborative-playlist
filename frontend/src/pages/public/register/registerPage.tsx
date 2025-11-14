import { useState } from "react";
import { Link } from "react-router-dom";
import { UserPlus } from "lucide-react";
import axios from "axios";
import styles from "./registerPage.module.css";

const RegisterPage = () => {
  const [values, setValues] = useState({
    username: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [errors, setErrors] = useState<any>({});
  const [message, setMessage] = useState("");

  //updates form values as the user types.
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    //e.target.name → matches the input’s name attribute (username, email, etc.).
    const { name, value } = e.target;
    //setValues → updates only the changed field while keeping others unchanged (...prev).
    setValues((prev) => ({ ...prev, [name]: value }));
    setErrors((prev: any) => ({ ...prev, [name]: "" }));
  };

  const validate = () => {
    const newErrors: any = {};
    if (!values.username.trim()) newErrors.firstName = "Username is required.";
    if (!values.email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/))
      newErrors.email = "Enter a valid email address.";
    if (!values.password || values.password.length < 6)
      newErrors.password = "Password must be at least 6 characters.";
    if (values.password !== values.confirmPassword)
      newErrors.confirmPassword = "Passwords do not match.";
    return newErrors;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); //e.preventDefault() → prevents the page from refreshing on submit (default browser behavior).

    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      setMessage("Registering...");
      await axios.post("http://localhost:5000/api/auth/register", {
        username: values.username,
        email: values.email, 
        password: values.password,
        role: "User",
      });
      setMessage("Account created successfully!");
      setValues({ username: "", email: "", password: "", confirmPassword: "" });
    } catch (err: any) {
      setMessage("");
      setErrors({ general: err.response?.data?.message || "Registration failed" });
    }
  };

  return (
    <div className={styles.loginPage}>
      <div className={styles.loginBox}>
        <h1 className={styles.header}>Create your account</h1>
        <p className={styles.text}>Start building collaborative playlists in seconds.</p>

        <form onSubmit={handleSubmit}>
            <div className={styles.formGroup}>
              <label>Username</label>
              <input
                name="username"
                value={values.username}
                onChange={handleChange}
                placeholder="Enter your username"
              />
              {errors.username && <p className={styles.error}>{errors.username}</p>}
            </div>
          <div className={styles.formGroup}>
            <label>Email</label>
            <input
              name="email"
              type="email"
              value={values.email}
              onChange={handleChange}
              placeholder="Enter your email"
            />
            {errors.email && <p className={styles.error}>{errors.email}</p>}
          </div>

          <div className={styles.formGroup}>
            <label>Password</label>
            <input
              name="password"
              type="password"
              value={values.password}
              onChange={handleChange}
              placeholder="Enter your password"
            />
            {errors.password && <p className={styles.error}>{errors.password}</p>}
          </div>

          <div className={styles.formGroup}>
            <label>Confirm Password</label>
            <input
              name="confirmPassword"
              type="password"
              value={values.confirmPassword}
              onChange={handleChange}
              placeholder="Confirm your password"
            />
            {errors.confirmPassword && <p className={styles.error}>{errors.confirmPassword}</p>}
          </div>

          <button className={styles.signInBtn} type="submit">
            <UserPlus className={styles.icon} size={18} />
            Sign Up
          </button>
        </form>

        {message && <p className={styles.message}>{message}</p>}
        {errors.general && <p className={styles.error}>{errors.general}</p>}

        <p className={styles.signupText}>
          <span className={styles.grey}>Already have an account? </span>
          <Link to="/login">Log in</Link>
        </p>
      </div>
    </div>
  );
};

export default RegisterPage;
