import { useState } from "react";
import "../styles/PlaylistModal.scss";
import CreatePlaylistForm from "./CreatePlaylist";

export default function PlaylistModal({ close }: { close: () => void }) {
    return (
      <div className="modal-overlay" onClick={close}>
        <div className="modal-box" onClick={(e) => e.stopPropagation()}>
          <h2>Create Playlist</h2>
          <CreatePlaylistForm />  
          <button onClick={close}>Close</button>
        </div>
      </div>
    );
  }