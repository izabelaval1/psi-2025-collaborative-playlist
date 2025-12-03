import { Music, Users } from "lucide-react";
import "./PlaylistCard.scss";
import type { Playlist } from "../../../../types/Playlist";

interface PlaylistCardProps {
  playlist: Playlist;
  onClick?: () => void;
}

export default function PlaylistCard({ playlist, onClick }: PlaylistCardProps) {
  return (
    <div className="playlist-card" onClick={onClick}>
      <div className="playlist-card__cover">
        <img
          src={`https://picsum.photos/seed/${playlist.id}/300`}
          alt={playlist.name}
          className="playlist-card__cover-image"
        />
        <div className="playlist-card__overlay">
          <button className="playlist-card__play-button">
            <Music size={24} />
          </button>
        </div>
      </div>

      <div className="playlist-card__info">
        <h3 className="playlist-card__title">{playlist.name}</h3>
        {playlist.description && (
          <p className="playlist-card__description">{playlist.description}</p>
        )}
        
        <div className="playlist-card__meta">
          <Users size={14} />
          <span>Collaborative</span>
        </div>
      </div>
    </div>
  );
}