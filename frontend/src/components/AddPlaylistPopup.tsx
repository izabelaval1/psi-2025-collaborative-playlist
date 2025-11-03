// src/components/AddPlaylistPopup.tsx
import { useState } from "react";
import CreatePlaylistForm from "./CreatePlaylist";

export default function AddPlaylistPopup({
  onPlaylistCreated,
}: {
  onPlaylistCreated: (playlist: any) => void;
}) {
  const [isOpen, setIsOpen] = useState(false);

  const openPopup = () => setIsOpen(true);
  const closePopup = () => setIsOpen(false);

  return (
    <>
      {/* Button */}
      <button
        onClick={openPopup}
        className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-full text-2xl font-bold"
        data-testid="add-playlist-button"
      >
        +
      </button>

      {/* Popup */}
      {isOpen && (
        <div
          className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50"
          data-testid="add-playlist-popup"
        >
          <div className="bg-white rounded-2xl p-6 shadow-lg w-full max-w-md relative">
            <button
              onClick={closePopup}
              className="absolute top-2 right-3 text-gray-500 hover:text-black"
              data-testid="add-playlist-popup-close"
            >
              ✕
            </button>

            <h2
              className="text-xl font-semibold mb-2"
              data-testid="add-playlist-popup-title"
            >
              Add Playlist
            </h2>

            <p
              className="text-gray-600 mb-4"
              data-testid="add-playlist-popup-description"
            >
              Fill out the form to create a new playlist.
            </p>

            <CreatePlaylistForm
              onPlaylistCreated={(playlist) => {
                onPlaylistCreated(playlist);
                closePopup();
              }}
            />
          </div>
        </div>
      )}
    </>
  );
}
