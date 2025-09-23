import { useState } from 'react'
import './App.css'
import PlaylistList from './components/PlaylistList'
import 'bootstrap/dist/css/bootstrap.min.css'
import { InputGroup, Form, Button } from 'react-bootstrap'
//import SearchBar from './components/SearchBar'
import PopUp from './components/PopUp'


function App() {

  const [isPopupOpen, setPopupOpen] = useState(false);

  return (
    <div>
      <>
      <Button variant="primary" onClick={() => setPopupOpen(true)}>
        Add a song
      </Button>

      <PopUp
        show={isPopupOpen}
        onHide={() => setPopupOpen(false)}
      />
    </>
      <h1>Playlists : </h1>
      <PlaylistList />
    </div>
  );
}

// function MyButton({ onClick }: { onClick: () => void }) {
//   return (
//     <button 
//       onClick={onClick}
//       className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold"
//     >
//       +
//     </button>
//   );
// }

// function MyPopup({ onClose }: { onClose: () => void }) {
//   return (
//     <div className="popup-overlay">
//       <div className="popup-content">
//         <h2>Popup content</h2>
//         <button onClick={onClose}>
//           Close
//         </button>
//       </div>
//     </div>
//   );
// }

export default App;
