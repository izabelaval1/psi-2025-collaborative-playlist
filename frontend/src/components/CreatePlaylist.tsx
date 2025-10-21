import { useState } from "react";
import type { Playlist } from "../types/Playlist";

interface CreatePlaylistFormProps {
  onPlaylistCreated: (newPlaylist: Playlist) => void;
}

export default function CreatePlaylistForm({ onPlaylistCreated }: CreatePlaylistFormProps) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [loading, setLoading] = useState(false);

  // Runs when the Create button is clicked
  const createPlaylist = async () => {
    if (!name.trim()) return alert("Please enter a playlist name.");
    setLoading(true);

    try {
      const res = await fetch("http://localhost:5000/api/playlists", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, description, songs: [] }),
      });

      if (!res.ok) throw new Error("Failed to create playlist");
      const newPlaylist: Playlist = await res.json();

      // ✅ Inform parent so it can refresh list
      onPlaylistCreated(newPlaylist);

      // Clear inputs
      setName("");
      setDescription("");
    } catch (err) {
      console.error("Error creating playlist:", err);
      alert("Failed to create playlist.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h2>Create new playlist</h2>
      <input
        placeholder="Playlist name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        disabled={loading}
      />
      <input
        placeholder="Description"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
        disabled={loading}
      />
      <button onClick={createPlaylist} disabled={loading}>
        {loading ? "Creating..." : "Create"}
      </button>
    </div>
  );
}
