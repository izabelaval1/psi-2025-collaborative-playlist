// src/components/PlaylistList.tsx
//playlist sidebar thingie
import {
  useState,
  useEffect,
  forwardRef,
  useImperativeHandle,
} from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit, faTrash } from "@fortawesome/free-solid-svg-icons";
import type { Playlist } from "../types/Playlist";
import {PlaylistService } from "../services/PlaylistService";
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
    const [editDescription, setEditDescription] = useState<string | undefined>(undefined);

    // Load playlists
    const loadPlaylists = async () => {
      try {
        const data = await PlaylistService.getAll();
        setPlaylists(data);
        onPlaylistsLoaded?.(data);
      } catch (err) {
        console.error("Error loading playlists:", err);
      }
    };

    useEffect(() => {
      loadPlaylists();
    }, []);

    useImperativeHandle(ref, () => ({ refresh: loadPlaylists }));

    // Delete playlist
    const deletePlaylist = async (id: number) => {
      if (!confirm("Delete this playlist?")) return;
      try {
        await PlaylistService.delete(id);
        const newPlaylists = playlists.filter((p) => p.id !== id);
        setPlaylists(newPlaylists);
        onPlaylistsLoaded?.(newPlaylists);
      } catch (err) {
        console.error("Error deleting playlist:", err);
        alert(err instanceof Error ? err.message : "Failed to delete playlist.");
      }
    };

    // Save edited playlist
    const saveEdit = async (playlistId: number) => {
      try {
        const updated = await PlaylistService.update(playlistId, {
          name: editName,
          description: editDescription,
        });
        const newPlaylists = playlists.map((p) =>
          p.id === updated.id ? updated : p
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
      <div className="playlist-container" data-testid="playlist-list__container">
        <h2 className="playlist-title" data-testid="playlist-list__title">
          Playlists:
        </h2>

        {playlists.length === 0 && (
          <p className="playlist-empty" data-testid="playlist-list__empty">
            No playlists yet — create one!
          </p>
        )}

        {playlists.map((playlist) => (
          <div
            key={playlist.id}
            className="playlist-card hover:bg-neutral-800 p-2 rounded cursor-pointer"
            data-testid={`playlist-list__card-${playlist.id}`}
            onClick={() => onSelect(playlist)}
          >
            {editingId === playlist.id ? (
              <div className="edit-mode" data-testid="playlist-list__edit-mode">
                <input
                  data-testid="playlist-list__edit-name"
                  value={editName}
                  onChange={(e) => setEditName(e.target.value)}
                  placeholder="Playlist name"
                  onClick={(e) => e.stopPropagation()}
                />
                <input
                  data-testid="playlist-list__edit-desc"
                  value={editDescription}
                  onChange={(e) => setEditDescription(e.target.value)}
                  placeholder="Description"
                  onClick={(e) => e.stopPropagation()}
                />
                <div className="playlist-buttons">
                  <button
                    data-testid="playlist-list__save-btn"
                    onClick={(e) => {
                      e.stopPropagation();
                      saveEdit(playlist.id);
                    }}
                  >
                    Save
                  </button>
                  <button
                    data-testid="playlist-list__cancel-btn"
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
                <div className="playlist-info" data-testid="playlist-list__info">
                  <h2 data-testid="playlist-list__name">{playlist.name}</h2>
                  <p data-testid="playlist-list__desc">{playlist.description}</p>
                </div>

                <div className="playlist-icons flex gap-3">
                  <FontAwesomeIcon
                    icon={faEdit}
                    className="icon text-blue-400 hover:text-blue-500"
                    title="Edit playlist"
                    data-testid="playlist-list__edit-icon"
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
                    data-testid="playlist-list__delete-icon"
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
