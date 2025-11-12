import { useState } from "react";
import type { Track } from "../types/Spotify";
import type { Playlist } from "../types/Playlist";
import { songService } from "../services/SongService";

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

  // Search songs
  const handleSearch = async () => {
    if (!query.trim()) {
      setResults([]);
      setIsSearching(false);
      return;
    }

    setIsSearching(true);
    setIsLoading(true);

    try {
      const tracks = await songService.search(query);
      setResults(tracks);
    } catch (err) {
      console.error("Search failed:", err);
      setResults([]);
    } finally {
      setIsLoading(false);
    }
  };

  // Add song to playlist
  const handleAddSong = async (track: Track) => {
    if (!selectedPlaylistId) {
      alert("Please select a playlist first!");
      return;
    }

    try {
      await songService.addToPlaylist(track, selectedPlaylistId);
      alert("Song added successfully!");
      setQuery("");
      setResults([]);
      setIsSearching(false);
      onSongAdded?.();
    } catch (err) {
      console.error("Failed to add song:", err);
      alert(err instanceof Error ? err.message : "Failed to add song.");
    }
  };

  return (
    <div className="px-6 pt-6">
      {/* Search Controls */}
      <div className="flex items-center gap-2">
        {/* Playlist Dropdown */}
        <select
          aria-label="Choose a playlist"
          data-testid="song-search-playlist-select"
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
          data-testid="song-search-input"
          type="text"
          placeholder="Search for songs..."
          className="bg-neutral-900 rounded-lg px-4 py-2 flex-1 text-white placeholder-gray-500 focus:outline-none border border-neutral-800"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSearch()}
        />

        <button
          data-testid="song-search-button"
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
                data-testid="song-add-btn"
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
