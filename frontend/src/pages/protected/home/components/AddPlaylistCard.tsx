import "./AddPlaylistCard.scss";

interface AddPlaylistCardProps {
  onClick?: () => void;
}

export default function AddPlaylistCard({ onClick }: AddPlaylistCardProps) {
  return (
    <div className="add-playlist-card" onClick={onClick}>
      <h3 className="add-playlist-card__title">Start a new collaborative playlist</h3>
      <p className="add-playlist-card__description">Create a shared playlist and invite friends.</p>
      <button type="button" className="add-playlist-card__button">
        New playlist
      </button>
    </div>
  );
}