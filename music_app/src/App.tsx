import { useState } from 'react'
import './App.css'

function App() {
  const [isPopupOpen, setPopupOpen] = useState(false);

  return (
    <div>
      <h1>Playlists : </h1>
      <MyButton onClick={() => setPopupOpen(true)} />
      {isPopupOpen && (
        <MyPopup onClose={() => setPopupOpen(false)} />
      )}
    </div>
  );
}

function MyButton({ onClick }: { onClick: () => void }) {
  return (
    <button onClick={onClick}>
      +
    </button>
  );
}

function MyPopup({ onClose }: { onClose: () => void }) {
  return (
    <div className="popup-overlay">
      <div className="popup-content">
        <h2>Popup content here!</h2>
        <button onClick={onClose}>
          Close
        </button>
      </div>
    </div>
  );
}

export default App;
