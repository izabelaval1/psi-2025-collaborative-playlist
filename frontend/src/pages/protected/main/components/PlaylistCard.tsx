import { Music, Users } from "lucide-react";
import "./PlaylistCard.scss";
import type { Playlist } from "../../../../types/Playlist";

interface PlaylistCardProps {
  playlist: Playlist;
  onClick?: () => void;
}

export default function PlaylistCard({ playlist, onClick }: PlaylistCardProps) {
  // Helper function to get the correct image URL
  const getImageUrl = () => {
    if (!playlist.imageUrl) {
      return `https://picsum.photos/seed/${playlist.id}/300`;
    }
    
    // If imageUrl is already a complete URL (starts with http)
    if (playlist.imageUrl.startsWith('http')) {
      return playlist.imageUrl;
    }
    
    // If it's a relative path, prepend the backend base URL
    const baseUrl = 'http://localhost:5000';
    const path = playlist.imageUrl.startsWith('/') ? playlist.imageUrl : `/${playlist.imageUrl}`;
    return `${baseUrl}${path}`;
  };

  return (
    <div className="playlist-card" onClick={onClick}>
      <div className="playlist-card__cover">
        <img
          src={getImageUrl()}
          alt={playlist.name}
          className="playlist-card__cover-image"
          onError={(e) => {
            console.error('Failed to load image:', getImageUrl());
            // Fallback if image fails to load
            e.currentTarget.src = `https://picsum.photos/seed/${playlist.id}/300`;
          }}
        />
        <div className="playlist-card__overlay">
          <button type="button" className="playlist-card__play-button">
            <Music size={24} /> +
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