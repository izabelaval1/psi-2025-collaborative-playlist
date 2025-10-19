import { useState } from "react";
import CreatePlaylistForm from "./CreatePlaylist";

interface PlaylistModalProps {
  close: () => void;
}

export default function PlaylistModal({ close }: PlaylistModalProps) {
  return (
    <div 
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" 
      onClick={close}
    >
      <div 
        className="bg-neutral-900 rounded-xl p-6 w-full max-w-md shadow-lg" 
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-white text-xl font-bold mb-4">Create Playlist</h2>
        <CreatePlaylistForm />
        <button
          className="mt-4 w-full bg-red-500 hover:bg-red-600 text-white py-2 rounded-lg"
          onClick={close}
        >
          Close
        </button>
      </div>
    </div>
  );
}
