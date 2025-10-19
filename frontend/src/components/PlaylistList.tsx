import React, { useState, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit, faTrash } from "@fortawesome/free-solid-svg-icons";
import type { PlaylistResponseDto } from "../types/PlaylistResponseDto.ts";

interface PlaylistListProps {
  onSelect: (playlist: PlaylistResponseDto) => void;
}

export default function PlaylistList({ onSelect }: PlaylistListProps) {
  const [playlists, setPlaylists] = useState<PlaylistResponseDto[]>([]);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editName, setEditName] = useState("");
  const [editDescription, setEditDescription] = useState("");

  // Load playlists from API
  useEffect(() => {
    fetch("http://localhost:5000/api/playlists")
      .then((res) => res.json())
      .then((data: PlaylistResponseDto[]) => setPlaylists(data))
      .catch((error) => {
        console.error("Failed to fetch playlists:", error);
        setPlaylists([]); // Set empty array on error
      });
  }, []);

  // Save edits
  const saveEdit = (playlistId: number) => {
    fetch(`http://localhost:5000/api/playlists/by-id/${playlistId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        id: playlistId,
        name: editName,
        description: editDescription,
        songs: playlists.find((p) => p.id === playlistId)?.songs || [],
      }),
    })
      .then((res) => res.json())
      .then((updatedPlaylist: PlaylistResponseDto) => {
        setPlaylists(
          playlists.map((p) => (p.id === playlistId ? updatedPlaylist : p))
        );
        setEditingId(null);
      })
      .catch((error) => console.error("Failed to save edit:", error));
  };

  // Delete playlist
  const deletePlaylist = (playlistId: number) => {
    fetch(`http://localhost:5000/api/playlists/${playlistId}`, {
      method: "DELETE",
    })
      .then(() => {
        setPlaylists(playlists.filter((p) => p.id !== playlistId));
      })
      .catch((error) => console.error("Failed to delete playlist:", error));
  };

  return (
    <div className="flex flex-col gap-4 bg-neutral-900 p-4 rounded-2xl h-full overflow-y-auto">
      <h2 className="text-xl font-bold text-white mb-2">Playlists</h2>

      {playlists.length === 0 && (
        <p className="text-gray-400 text-sm">No playlists found</p>
      )}

      {playlists.map((playlist) => (
        <div
          key={playlist.id}
          className="bg-neutral-700 hover:bg-neutral-600 p-3 rounded-lg cursor-pointer transition-colors"
          onClick={() => onSelect(playlist)}
        >
          {editingId === playlist.id ? (
            <div className="flex flex-col gap-2 w-full">
              <input
                className="px-2 py-1 rounded bg-neutral-800 text-white w-full"
                value={editName}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setEditName(e.target.value)
                }
                placeholder="Playlist Name"
              />
              <input
                className="px-2 py-1 rounded bg-neutral-800 text-white w-full"
                value={editDescription}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setEditDescription(e.target.value)
                }
                placeholder="Playlist Description"
              />
              <div className="flex gap-2 mt-1">
                <button
                  className="bg-green-500 text-black px-3 py-1 rounded hover:bg-green-600"
                  onClick={(e) => {
                    e.stopPropagation();
                    saveEdit(playlist.id);
                  }}
                >
                  Save
                </button>
                <button
                  className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
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
            <div className="flex flex-col gap-2">
              <div className="flex flex-col">
                <span className="text-white font-semibold">
                  {playlist.name}
                </span>
                <span className="text-gray-400 text-sm line-clamp-2">
                  {playlist.description || "No description"}
                </span>
              </div>

              <div className="flex gap-3 mt-2">
                <FontAwesomeIcon
                  icon={faEdit}
                  className="text-gray-300 hover:text-white cursor-pointer"
                  onClick={(e: React.MouseEvent<SVGSVGElement, MouseEvent>) => {
                    e.stopPropagation();
                    setEditingId(playlist.id);
                    setEditName(playlist.name);
                    setEditDescription(playlist.description ?? "");
                  }}
                />
                <FontAwesomeIcon
                  icon={faTrash}
                  className="text-gray-300 hover:text-red-500 cursor-pointer"
                  onClick={(e: React.MouseEvent<SVGSVGElement, MouseEvent>) => {
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