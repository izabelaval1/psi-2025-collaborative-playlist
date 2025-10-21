import { useState } from "react";
import type { Track, SpotifyResponse } from "./Spotify";
import type { Playlist } from "../types/Playlist";

interface SongSearchProps {
  onSongAdded?: () => void;
  playlists: Playlist[];
}

export default function SongSearch({ onSongAdded, playlists }: SongSearchProps) {
  const [selectedPlaylistId, setSelectedPlaylistId] = useState<number | null>(null);
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<Track[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleSearch = async () => {
    if (!query.trim()) {
      setResults([]);
      setIsSearching(false);
      return;
    }

    setIsSearching(true);
    setIsLoading(true);

    try {
      const response = await fetch(
        `http://localhost:5000/api/Spotify/search/${encodeURIComponent(query)}`
      );
      const data: SpotifyResponse = await response.json();
      setResults(data.tracks?.items || []);
    } catch (error) {
      console.error("Search failed:", error);
      setResults([]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleAddSong = async (track: Track) => {
    if (!selectedPlaylistId) {
      alert("Please select a playlist first!");
      return;
    }

    const songData = {
      PlaylistId: selectedPlaylistId,
      Title: track.name,
      Artist: track.artists.map((a) => a.name).join(", "),
      Album: track.album.name,
      Url: track.external_urls.spotify,
    };

    try {
      const response = await fetch(`http://localhost:5000/api/songs/add-to-playlist`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(songData),
      });

      if (response.status === 409) {
        alert("This song is already in the playlist!");
        return;
      }

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Failed to add song: ${errorText}`);
      }

      alert("Song added successfully!");
      setQuery("");
      setResults([]);
      setIsSearching(false);
      onSongAdded?.();
    } catch (error) {
      console.error("Failed to add song:", error);
      alert(`Failed to add song: ${error}`);
    }
  };

  return (
    <div className="px-6 pt-6">
      {/* Search Controls */}
      <div className="flex items-center gap-2">
        {/* Playlist Dropdown */}
        <select
          value={selectedPlaylistId ?? ""}
          onChange={(e) => setSelectedPlaylistId(Number(e.target.value))}
          className="bg-neutral-900 text-white rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-green-500 border border-neutral-800"
        >
          <option value="" className="bg-neutral-900 text-gray-400">
            -- Select playlist --
          </option>
          {playlists.map((p) => (
            <option key={p.id} value={p.id} className="bg-neutral-900 text-white">
              {p.name}
            </option>
          ))}
        </select>

        {/* Search Input */}
        <input
          type="text"
          placeholder="Search for songs..."
          className="bg-neutral-900 rounded-lg px-4 py-2 flex-1 text-white placeholder-gray-500 focus:outline-none border border-neutral-800"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSearch()}
        />

        <button
          type="button"
          onClick={handleSearch}
          disabled={isLoading}
          className="bg-green-500 hover:bg-green-600 disabled:opacity-50 text-white px-4 py-2 rounded-lg font-semibold"
        >
          {isLoading ? "..." : "🔍"}
        </button>
      </div>

      {/* Results */}
      {isSearching && results.length > 0 && (
        <div className="mt-4 bg-neutral-900 rounded-lg shadow-lg max-h-96 overflow-y-auto border border-neutral-800">
          {results.map((track) => (
            <div
              key={track.id}
              className="p-4 hover:bg-neutral-800 border-b border-neutral-800 last:border-b-0 flex justify-between"
            >
              <div>
                <div className="font-semibold text-white">{track.name}</div>
                <div className="text-gray-400 text-sm">
                  {track.artists.map((a) => a.name).join(", ")}
                </div>
                <div className="text-gray-500 text-sm">{track.album.name}</div>
                <a
                  href={track.external_urls.spotify}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-green-400 hover:text-green-300 underline text-sm mt-1 inline-block"
                >
                  Listen on Spotify
                </a>
              </div>

              <button
                className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-lg font-semibold"
                onClick={() => handleAddSong(track)}
              >
                + Add
              </button>
            </div>
          ))}
        </div>
      )}

      {isSearching && results.length === 0 && !isLoading && (
        <p className="text-gray-500 mt-4 text-center">No songs found.</p>
      )}
    </div>
  );
}