import {Sparkles, Plus, Play, UsersRound, Search, MessageCircle} from 'lucide-react';
import styles from "./landingPage.module.css"
import { useNavigate } from "react-router-dom";

export default function LandingPage() {
  const navigate = useNavigate();

    return (
      <div className={styles.landing}>
        <div className={styles.heroDiv}>
          <Sparkles size={18} strokeWidth={2} className={styles.sparklesIcon}/>
          Collaborative playlists
        </div>
        <h1 className={styles.header}>Your Music, Together. Instantly.</h1>
        <h3 className={styles.desc}>Create, share, and collaborate on playlists with friends. Search songs, drag to add, and listen as a group.</h3>
        <div className={styles.landingBtns}>
          <button 
          className={styles.getStartedBtn} 
          type="button"
          onClick={() => navigate("/register")}>
            <Play strokeWidth={2} size={18} className={styles.playIcon}/>
            Get started
    
            </button>
          <button  className={styles.startPlaylistBtn} type="button">
            <Plus className={styles.plusIcon} strokeWidth={2} size={18}/>
            Start a playlist</button>
        </div>
        <div className={styles.landingPerks}>
          <div>
            <UsersRound size={18} className={styles.icon}/>
            <h3>Invite with one link</h3>
            <p>Share and start adding tracks in seconds.</p>
          </div>
          <div>
            <MessageCircle size={18} className={styles.icon}/>
            <h3>Reactions</h3>
            <p>Drop emojis and comments on tracks.</p>
          </div>
          <div>
            < Search size={18} className={styles.icon}/>
            <h3>Powerful Search</h3>
            <p>Find songs fast across genres and moods.</p>
          </div>
        </div>
      </div>
    );
  }