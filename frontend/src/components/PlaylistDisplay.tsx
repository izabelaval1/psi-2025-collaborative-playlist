import React from "react";
import type { PlaylistResponseDto } from "../types/PlaylistResponseDto.ts";



interface PlaylistDisplayProps {
  playlist: PlaylistResponseDto | null;
}

const formatDuration = (seconds?: number): string => {
  if (!seconds) return "--:--";
  const mins = Math.floor(seconds / 60);
  const secs = seconds % 60;
  return `${mins}:${secs.toString().padStart(2, "0")}`;
};

const PlaylistDisplay: React.FC<PlaylistDisplayProps> = ({ playlist }) => {
  if (!playlist) {
    return (
      <div className="flex items-center justify-center text-gray-400 text-lg bg-neutral-900 rounded-2xl p-6 h-full">
        Select a playlist to view its details.
      </div>
    );
  }

  return (
    <div className="bg-neutral-900 text-white p-6 rounded-2xl shadow-lg overflow-y-auto h-full">
      {/* Header */}
      <div className="flex flex-col md:flex-row items-center mb-8">
        <img
          src={`https://picsum.photos/seed/${playlist.id}/200`}
          alt={playlist.name}
          className="w-48 h-48 rounded-xl object-cover mb-4 md:mb-0 md:mr-6 shadow-md"
        />

        <div className="flex flex-col">
          <h1 className="text-3xl font-bold mb-2">{playlist.name}</h1>
          <p className="text-gray-400 mb-4">
            {playlist.description || "No description"}
          </p>

          <div className="flex flex-wrap gap-2">
            {playlist.collaborators.map((user) => (
              <span
                key={user.id}
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
      <div className="overflow-x-auto">
        <table className="min-w-full text-sm border-separate border-spacing-y-2">
          <thead>
            <tr className="text-gray-400 border-b border-neutral-700">
              <th className="text-left py-2 px-4">#</th>
              <th className="text-left py-2 px-4">Title</th>
              <th className="text-left py-2 px-4">Artist(s)</th>
              <th className="text-left py-2 px-4">Album</th>
              <th className="text-left py-2 px-4">Duration</th>
            </tr>
          </thead>
          <tbody>
            {playlist.songs.map((song, index) => (
              <tr
                key={song.id}
                className="hover:bg-neutral-800 rounded-lg transition-colors"
              >
                <td className="py-2 px-4 text-gray-400">{index + 1}</td>
                <td className="py-2 px-4 font-medium">{song.title}</td>
                <td className="py-2 px-4 text-gray-300">
                  {song.artists.map((a) => a.name).join(", ")}
                </td>
                <td className="py-2 px-4 text-gray-400">{song.album || "—"}</td>
                <td className="py-2 px-4 text-gray-400">
                  {formatDuration(song.duration)}
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