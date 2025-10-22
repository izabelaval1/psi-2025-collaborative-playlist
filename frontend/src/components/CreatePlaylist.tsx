import { useState } from "react";
import type { Playlist } from "../types/Playlist";

interface CreatePlaylistFormProps {
  onPlaylistCreated: (newPlaylist: Playlist) => void;
}

export default function CreatePlaylistForm({ onPlaylistCreated }: CreatePlaylistFormProps) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [hostId, setHostId] = useState("1"); // Default host ID
  const [loading, setLoading] = useState(false);

  const createPlaylist = async () => {
    if (!name.trim()) return alert("Please enter a playlist name.");
    if (!hostId.trim()) return alert("Please enter a host ID.");
    
    setLoading(true);

    try {
      const res = await fetch("http://localhost:5000/api/playlists", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ 
          name, 
          description, 
          hostId: parseInt(hostId) 
        }),
      });

      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(errorText || "Failed to create playlist");
      }
      
      const newPlaylist: Playlist = await res.json();
      onPlaylistCreated(newPlaylist);

      // Clear inputs
      setName("");
      setDescription("");
    } catch (err) {
      console.error("Error creating playlist:", err);
      alert(err instanceof Error ? err.message : "Failed to create playlist.");
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
      <input
        placeholder="Host ID"
        type="number"
        value={hostId}
        onChange={(e) => setHostId(e.target.value)}
        disabled={loading}
      />
      <button onClick={createPlaylist} disabled={loading}>
        {loading ? "Creating..." : "Create"}
      </button>
    </div>
  );
}