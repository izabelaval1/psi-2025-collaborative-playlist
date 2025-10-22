import {
  useState,
  useEffect,
  forwardRef,
  useImperativeHandle,
} from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit, faTrash } from "@fortawesome/free-solid-svg-icons";
import type { Playlist } from "../types/Playlist";
import "../styles/Playlists.scss";

interface PlaylistListProps {
  onSelect: (playlist: Playlist) => void;
  onPlaylistsLoaded?: (playlists: Playlist[]) => void;
}

export interface PlaylistListHandle {
  refresh: () => Promise<void>;
}

const PlaylistList = forwardRef<PlaylistListHandle, PlaylistListProps>(
  ({ onSelect, onPlaylistsLoaded }, ref) => {
    const [playlists, setPlaylists] = useState<Playlist[]>([]);
    const [editingId, setEditingId] = useState<number | null>(null);
    const [editName, setEditName] = useState("");
    const [editDescription, setEditDescription] = useState("");

    const loadPlaylists = async () => {
      try {
        const res = await fetch("http://localhost:5000/api/playlists");
        if (!res.ok) throw new Error("Failed to load playlists");
        const data = await res.json();
        setPlaylists(data);
        onPlaylistsLoaded?.(data);
      } catch (err) {
        console.error("Error loading playlists:", err);
      }
    };

    useEffect(() => {
      loadPlaylists();
    }, []);

    useImperativeHandle(ref, () => ({
      refresh: loadPlaylists,
    }));

    const deletePlaylist = async (id: number) => {
      if (!confirm("Delete this playlist?")) return;
      try {
        const res = await fetch(`http://localhost:5000/api/playlists/${id}`, {
          method: "DELETE",
        });
        
        if (!res.ok) {
          const errorText = await res.text();
          throw new Error(errorText || "Delete failed");
        }
        
        const newPlaylists = playlists.filter((p) => p.id !== id);
        setPlaylists(newPlaylists);
        onPlaylistsLoaded?.(newPlaylists);
      } catch (err) {
        console.error("Error deleting playlist:", err);
        alert(err instanceof Error ? err.message : "Failed to delete playlist.");
      }
    };

    const saveEdit = async (playlistId: number) => {
      try {
        const currentPlaylist = playlists.find((p) => p.id === playlistId);
        
        // Use PATCH for partial update (name and description only)
        const res = await fetch(
          `http://localhost:5000/api/playlists/${playlistId}`,
          {
            method: "PATCH",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
              name: editName,
              description: editDescription,
            }),
          }
        );

        if (!res.ok) {
          const errorText = await res.text();
          throw new Error(errorText || "Failed to save edit");
        }
        
        const updatedPlaylist = await res.json();

        const newPlaylists = playlists.map((p) =>
          p.id === updatedPlaylist.id ? updatedPlaylist : p
        );
        setPlaylists(newPlaylists);
        onPlaylistsLoaded?.(newPlaylists);

        setEditingId(null);
      } catch (err) {
        console.error("Error updating playlist:", err);
        alert(err instanceof Error ? err.message : "Failed to update playlist.");
      }
    };

    return (
      <div className="playlist-container">
        <h2 className="playlist-title">Playlists:</h2>

        {playlists.length === 0 && (
          <p className="playlist-empty">No playlists yet — create one!</p>
        )}

        {playlists.map((playlist) => (
          <div
            key={playlist.id}
            className="playlist-card hover:bg-neutral-800 p-2 rounded cursor-pointer"
            onClick={() => onSelect(playlist)}
          >
            {editingId === playlist.id ? (
              <div className="edit-mode">
                <input
                  value={editName}
                  onChange={(e) => setEditName(e.target.value)}
                  placeholder="Playlist name"
                  onClick={(e) => e.stopPropagation()}
                />
                <input
                  value={editDescription}
                  onChange={(e) => setEditDescription(e.target.value)}
                  placeholder="Description"
                  onClick={(e) => e.stopPropagation()}
                />
                <div className="playlist-buttons">
                  <button 
                    onClick={(e) => {
                      e.stopPropagation();
                      saveEdit(playlist.id);
                    }}
                  >
                    Save
                  </button>
                  <button 
                    onClick={(e) => {
                      e.stopPropagation();
                      setEditingId(null);
                    }}
                  >
                    Cancel
                  </button>
                </div>
              </div>
            ) : (
              <div className="flex justify-between items-center">
                <div className="playlist-info">
                  <h2 className="playlist-name">{playlist.name}</h2>
                  <p className="playlist-desc">
                    {playlist.description}
                  </p>
                </div>

                <div className="playlist-icons flex gap-3">
                  <FontAwesomeIcon
                    icon={faEdit}
                    className="icon text-blue-400 hover:text-blue-500"
                    title="Edit playlist"
                    onClick={(e) => {
                      e.stopPropagation();
                      setEditingId(playlist.id);
                      setEditName(playlist.name);
                      setEditDescription(playlist.description);
                    }}
                  />

                  <FontAwesomeIcon
                    icon={faTrash}
                    className="icon text-red-400 hover:text-red-500"
                    title="Delete playlist"
                    onClick={(e) => {
                      e.stopPropagation();
                      deletePlaylist(playlist.id);
                    }}
                  />
                </div>
              </div>
            )}
          </div>
        ))}
      </div>
    );
  }
);

PlaylistList.displayName = "PlaylistList";

export default PlaylistList;