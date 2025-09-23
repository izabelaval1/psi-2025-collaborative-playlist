
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
      >
        + 
      </button>
    );
  }
  
  function Popup({ onClose }: AddPlaylistPopupProps) {
    return (
      <div className="popup-overlay">
        <div className="popup-content">
          <h2>Popup content</h2>
          <button onClick={onClose}>Close</button>
        </div>
      </div>
    );
  }
  

  const AddPlaylistPopup = { Button, Popup };
  export default AddPlaylistPopup;
  