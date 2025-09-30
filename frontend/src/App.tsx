import { useState } from 'react'
import './styles/App.css'
import PlaylistList from './components/PlaylistList'
import 'bootstrap/dist/css/bootstrap.min.css'
import { Button } from 'react-bootstrap'

import AddPlaylistPopup from "./components/AddPlaylistPopup"
import SearchSongModal from './components/SearchSongModal'
import PlaylistModal from './components/PlaylistModal'

function App() {
  const [isAddOpen, setAddOpen] = useState(false)     // for AddPlaylistPopup
  const [isSearchOpen, setSearchOpen] = useState(false) // for SearchSongModal
  const [showModal, setShowModal] = useState(false); //for creating playlist modal

  return (
    <div className="main-page">
      <aside className='left-side'>
        <PlaylistList />
      </aside>

      <div className="vDivider"></div>

      <div className='right-side'>
        <input type="text" name="search-bar" id="search-bar" placeholder='Mock search bar'/>
        <div className='home-page-btns'>
          {/* First button + its popup */}
          <div className='buttons-up'>
            <div className="popup-btn">
              <AddPlaylistPopup.Button onClick={() => setAddOpen(true)} />
              {isAddOpen && <AddPlaylistPopup.Popup onClose={() => setAddOpen(false)} />}
            </div>
            {/* Second button + its modal */}
            <div>
              <Button variant="primary" onClick={() => setSearchOpen(true)}>
                Add a song
              </Button>
              <SearchSongModal
                show={isSearchOpen}
                onHide={() => setSearchOpen(false)}
              />
            </div>
          </div>
          <div className='buttons-down'>
            <button className="add-playlist-btn" onClick={() => setShowModal(true)}>+ New Playlist</button>
            {showModal && <PlaylistModal close={() => setShowModal(false)} />}
          </div>

        </div>
      </div>

      
    </div>
  )
}

export default App



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
