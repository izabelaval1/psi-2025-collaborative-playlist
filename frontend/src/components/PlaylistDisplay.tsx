import React from "react";
import type { PlaylistResponseDto } from "../types/PlaylistResponseDto.ts";
import {PlaylistService } from "../services/PlaylistService.ts";

interface PlaylistDisplayProps {
  playlist: PlaylistResponseDto | null;
  onSongRemoved: () => void;
}

const formatDuration = (seconds?: number): string => {
  if (!seconds) return "--:--";
  const mins = Math.floor(seconds / 60);
  const secs = seconds % 60;
  return `${mins}:${secs.toString().padStart(2, "0")}`;
};

const PlaylistDisplay: React.FC<PlaylistDisplayProps> = ({ playlist, onSongRemoved }) => {
  const removeFromPlaylist = async (playlistId: number, songId: number) => {
    console.log("Deleting song ID:", songId);
    try {
      await PlaylistService.removeFromPlaylist(playlistId, songId);
      alert("Song deleted!");
      
      // Call the callback to refresh data in parent
      if (onSongRemoved) {
        onSongRemoved();
      }
    } catch (err) {
      alert("Failed to delete song: " + err);
    }
  };
  if (!playlist) {
    return (
      <div
        className="flex items-center justify-center text-gray-400 text-lg bg-neutral-900 rounded-2xl p-6 h-full"
        data-testid="playlist-display-empty"
      >
        Select a playlist to view its details.
      </div>
    );
  }

  return (
    <div
      className="bg-neutral-900 text-white p-6 rounded-2xl shadow-lg overflow-y-auto h-full"
      data-testid="playlist-display"
    >
      {/* Header */}
      <div
        className="flex flex-col md:flex-row items-center mb-8"
        data-testid="playlist-display-header"
      >
        <img
          src={`https://picsum.photos/seed/${playlist.id}/200`}
          alt={playlist.name}
          className="w-48 h-48 rounded-xl object-cover mb-4 md:mb-0 md:mr-6 shadow-md"
          data-testid="playlist-display-image"
        />

        <div className="flex flex-col" data-testid="playlist-display-meta">
          <h1
            className="text-3xl font-bold mb-2"
            data-testid="playlist-display-title"
          >
            {playlist.name}
          </h1>
          <p
            className="text-gray-400 mb-4"
            data-testid="playlist-display-description"
          >
            {playlist.description || "No description"}
          </p>

          <div
            className="flex flex-wrap gap-2"
            data-testid="playlist-display-collaborators"
          >
            {playlist.collaborators.map((user) => (
              <span
                key={user.id}
                data-testid={`playlist-display-collab-${user.id}`}
                className={`px-3 py-1 rounded-full text-sm font-medium ${
                  user.id === playlist.hostId
                    ? "bg-green-500 text-black font-semibold"
                    : "bg-neutral-700 text-gray-200"
                }`}
              >
                {user.username}
                {user.id === playlist.hostId && " (Host)"}
              </span>
            ))}
          </div>
        </div>
      </div>

      {/* Songs */}
      <div
        className="overflow-x-auto"
        data-testid="playlist-display-songs-container"
      >
        <table
          className="min-w-full text-sm border-separate border-spacing-y-2"
          data-testid="playlist-display-table"
        >
          <thead>
            <tr
              className="text-gray-400 border-b border-neutral-700"
              data-testid="playlist-display-table-header"
            >
              <th>#</th>
              <th className="px-4">Title</th>
              <th className="px-4">Artist(s)</th>
              <th className="px-4">Album</th>
              <th className="px-2">Duration</th>
              <th className="px-2">Delete</th>
            </tr>
          </thead>
          <tbody>
            {playlist.songs.map((song, index) => (
              <tr
                key={song.id}
                className="hover:bg-neutral-800 rounded-lg transition-colors"
                data-testid={`playlist-display-song-${song.id}`}
              >
                <td data-testid={`playlist-display-song-index-${song.id}`}>
                  {index + 1}
                </td>
                <td className="px-2" data-testid={`playlist-display-song-title-${song.id}`}>
                  {song.title}
                </td>
                <td data-testid={`playlist-display-song-artists-${song.id}`}>
                  {song.artists.map((a) => a.name).join(", ")}
                </td>
                <td data-testid={`playlist-display-song-album-${song.id}`}>
                  {song.album || "—"}
                </td>
                <td className="px-4" data-testid={`playlist-display-song-duration-${song.id}`}>
                  {song.durationFormatted || formatDuration(song.duration)}
                </td>
                <td className="px-3" data-testid="trash">
                  <button
                    onClick={() => removeFromPlaylist(playlist.id, song.id)}
                  >
                    🗑️
                  </button>

                  {/* how and where do i change the style */}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PlaylistDisplay;