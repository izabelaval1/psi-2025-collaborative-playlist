
import { useState } from "react";
import type { Playlist } from "../types/Playlist";
import {PlaylistService } from "../services/PlaylistService";

interface CreatePlaylistProps {
  onPlaylistCreated: (newPlaylist: Playlist) => void;
}

export default function CreatePlaylist({ onPlaylistCreated }: CreatePlaylistProps) {
  const [form, setForm] = useState({
    name: "",
    description: "",
    hostId: "1",
  });
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [imageFile, setImageFile] = useState<File | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setError(null);
    setSuccess(false);
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  const file = e.target.files?.[0] || null;
  setImageFile(file);
  setError(null);
  setSuccess(false);
  };

  const handleSubmit = async () => {
    const { name, description, hostId } = form;

    if (!name.trim()) return setError("Please enter a playlist name.");
    // if (!hostId.trim() || Number(hostId) <= 0)
    //   return setError("Host ID must be a valid positive number.");

    setLoading(true);
    setError(null);
    setSuccess(false);

    try {
      const newPlaylist = await PlaylistService.create({
        name: name.trim(),
        description: description.trim(),
        hostId: Number(hostId),
        imageFile: imageFile ?? undefined,
      });

      onPlaylistCreated(newPlaylist);
      setForm({ name: "", description: "", hostId }); // reset except hostId
      setImageFile(null);
      setSuccess(true);
    } catch (err: any) {
      setError(err.message || "Failed to create playlist.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4 border rounded-2xl shadow-sm max-w-md">
      <h2 className="text-lg font-semibold mb-3">Create New Playlist</h2>

      <input
        name="name"
        placeholder="Playlist name"
        value={form.name}
        onChange={handleChange}
        disabled={loading}
        className="w-full mb-2 p-2 border rounded-lg"
      />

      <input
        name="description"
        placeholder="Description"
        value={form.description}
        onChange={handleChange}
        disabled={loading}
        className="w-full mb-2 p-2 border rounded-lg"
      />

      <input
      type="file"
      accept="image/*"
      onChange={handleFileChange}
      disabled={loading}
      className="w-full mb-3 p-2 border rounded-lg"
      />


      <input
        name="hostId"
        placeholder="Host ID"
        type="number"
        min="1"
        value={form.hostId}
        onChange={handleChange}
        disabled={loading}
        className="w-full mb-3 p-2 border rounded-lg"
      />

      {error && <p className="text-red-500 text-sm mb-2">{error}</p>}
      {success && <p className="text-green-500 text-sm mb-2">Playlist created successfully!</p>}

      <button
        onClick={handleSubmit}
        disabled={loading}
        className="bg-blue-500 text-white px-4 py-2 rounded-lg w-full hover:bg-blue-600 disabled:opacity-60"
      >
        {loading ? "Creating..." : "Create Playlist"}
      </button>
    </div>
  );
}
