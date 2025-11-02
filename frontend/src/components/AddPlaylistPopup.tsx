interface AddPlaylistButtonProps {
  onClick: () => void;
}

interface AddPlaylistPopupProps {
  onClose: () => void;
}

function Button({ onClick }: AddPlaylistButtonProps) {
  return (
    <button
      onClick={onClick}
      className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold"
      data-testid="add-playlist-button"
    >
      +
    </button>
  );
}

function Popup({ onClose }: AddPlaylistPopupProps) {
  return (
    <div className="popup-overlay" data-testid="add-playlist-popup">
      <div className="popup-content" data-testid="add-playlist-popup-content">
        <h2 data-testid="add-playlist-popup-title">Add Playlist</h2>
        <p data-testid="add-playlist-popup-description">
          Fill out the form to create a new playlist.
        </p>
        <button onClick={onClose} data-testid="add-playlist-popup-close">
          Close
        </button>
      </div>
    </div>
  );
}

const AddPlaylistPopup = { Button, Popup };
export default AddPlaylistPopup;
