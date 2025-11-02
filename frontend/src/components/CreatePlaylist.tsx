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
    // Validacijos
    if (!name.trim()) {
      alert("Please enter a playlist name.");
      return;
    }
    
    if (!hostId.trim()) {
      alert("Please enter a host ID.");
      return;
    }
    
    // Konvertuoti hostId į skaičių ir patikrinti
    const hostIdNumber = parseInt(hostId, 10);
    if (isNaN(hostIdNumber) || hostIdNumber <= 0) {
      alert("Host ID must be a valid positive number.");
      return;
    }
    
    setLoading(true);

    try {
      const res = await fetch("http://localhost:5000/api/playlists", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ 
          name: name.trim(), 
          description: description.trim(), 
          hostId: hostIdNumber  // Naudojame tikrą skaičių
        }),
      });

      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(errorText || "Failed to create playlist");
      }
      
      const newPlaylist: Playlist = await res.json();
      onPlaylistCreated(newPlaylist);

      // Išvalyti inputus po sėkmingo sukūrimo
      setName("");
      setDescription("");
      // hostId paliekame - dažniausiai tas pats vartotojas kuria kelis playlist'us
      
      alert("Playlist created successfully!");
      
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
        min="1"
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