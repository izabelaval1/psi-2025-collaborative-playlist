import { Link } from "react-router-dom";
import PlaylistCard from "./PlaylistCard";
import AddPlaylistCard from "./AddPlaylistCard";
import "./RecentPlaylists.scss";
import type { Playlist } from "../../../../types/Playlist";

interface RecentPlaylistsProps {
  playlists: Playlist[];
  onPlaylistClick?: (playlist: Playlist) => void;
}

export default function RecentPlaylists({ playlists, onPlaylistClick }: RecentPlaylistsProps) {
  
  const recentPlaylists = [...playlists]
    .sort((a, b) => b.id - a.id) // Newest playlists first
    .slice(0, 3);

  return (
    <section className="recent-playlists">
      <div className="recent-playlists__header">
        <div>
          <h2 className="recent-playlists__title">Recent playlists</h2>
          <p className="recent-playlists__subtitle">
            Jump back into something you and your friends were playing.
          </p>
        </div>
        {playlists.length > 3 && (
          <Link to="/playlists" className="recent-playlists__view-all">
            View all
          </Link>
        )}
      </div>

      <div className="recent-playlists__grid">
        <AddPlaylistCard />
        
        {recentPlaylists.map((playlist) => (
          <PlaylistCard
            key={playlist.id}
            playlist={playlist}
            onClick={() => onPlaylistClick?.(playlist)}
          />
        ))}
      </div>
    </section>
  );
}