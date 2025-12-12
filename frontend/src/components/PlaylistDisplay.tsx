import React from "react";
import type { PlaylistResponseDto } from "../types/PlaylistResponseDto";
import { PlaylistService } from "../services/PlaylistService";
import { useSpotifyPlayer } from "../context/SpotifyPlayerContext";

interface PlaylistDisplayProps {
  playlist: PlaylistResponseDto | null;
  onSongRemoved: () => void;
}

const API_BASE = import.meta.env.VITE_API_URL;

const formatDuration = (seconds?: number): string => {
  if (!seconds) return "--:--";
  const mins = Math.floor(seconds / 60);
  const secs = seconds % 60;
  return `${mins}:${secs.toString().padStart(2, "0")}`;
};

const PlaylistDisplay: React.FC<PlaylistDisplayProps> = ({
  playlist,
  onSongRemoved,
}) => {
  const { play, pause, playerState, spotifyToken, deviceId } = useSpotifyPlayer();

  const removeFromPlaylist = async (playlistId: number, songId: number) => {
    try {
      await PlaylistService.removeFromPlaylist(playlistId, songId);
      alert("Song deleted!");
      onSongRemoved();
    } catch (err) {
      alert("Failed to delete song: " + err);
    }
  };

  const handlePlaySong = async (uri: string) => {
    if (!uri) {
      alert("Cannot play - missing Spotify URI");
      return;
    }

    if (!spotifyToken || !deviceId || !playerState.isReady) {
      return;
    }

    const isCurrent = playerState.currentTrackUri === uri;

    if (isCurrent && playerState.isPlaying) {
      await pause();
      return;
    }

    await play(uri, "track");
  };

  if (!playlist) {
    return (
      <div className="flex items-center justify-center text-gray-400 text-lg bg-neutral-900 rounded-2xl p-6 h-full">
        Select a playlist to view its details.
      </div>
    );
  }

  if (!spotifyToken || !deviceId || !playerState.isReady) {
    return <div className="text-gray-400 p-6">Spotify player is loading…</div>;
  }

  const coverSrc = playlist.imageUrl
    ? playlist.imageUrl.startsWith("http")
      ? playlist.imageUrl
      : `${API_BASE}${playlist.imageUrl}`
    : `https://picsum.photos/seed/${playlist.id}/200`;

  return (
    <div className="bg-neutral-900 text-white p-6 rounded-2xl shadow-lg overflow-y-auto h-full">
      {/* Header */}
      <div className="flex flex-col md:flex-row items-center mb-8">
        <div
          className="rounded-xl overflow-hidden mb-4 md:mb-0 md:mr-6 shadow-md flex-shrink-0"
          style={{ width: "192px", height: "192px" }}
        >
          <img
            src={coverSrc}
            alt={playlist.name}
            className="w-full h-full object-cover object-center block"
          />
        </div>

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
              <th className="px-2">#</th>
              <th className="px-2">Title</th>
              <th className="px-2">Artist(s)</th>
              <th className="px-4">Album</th>
              <th className="px-2">Duration</th>
              <th className="px-2">Actions</th>
            </tr>
          </thead>
          <tbody>
            {playlist.songs.map((song, index) => {
              const isPlaying = playerState.currentTrackUri === song.spotifyUri;
              const hasValidUri = song.spotifyUri && song.spotifyUri !== "";

              return (
                <tr
                  key={song.id}
                  className={`hover:bg-neutral-800 rounded-lg transition-colors ${
                    isPlaying ? "bg-neutral-800" : ""
                  }`}
                >
                  <td className="px-2">
                    {isPlaying && playerState.isPlaying ? (
                      <span className="text-green-500">▶</span>
                    ) : (
                      index + 1
                    )}
                  </td>

                  <td className={`px-2 ${isPlaying ? "text-green-500" : ""}`}>
                    {song.title}
                  </td>

                  <td className="px-4">
                    {song.artists.map((a) => a.name).join(", ")}
                  </td>

                  <td>{song.album || "—"}</td>

                  <td className="px-4">
                    {song.durationFormatted || formatDuration(song.duration)}
                  </td>

                  <td className="px-3">
                    <div className="flex gap-2">
                      <button
                        onClick={() => handlePlaySong(song.spotifyUri)}
                        className="hover:scale-110 transition-transform text-green-500 disabled:text-gray-600"
                        disabled={!hasValidUri}
                      >
                        {isPlaying && playerState.isPlaying ? "⏸️" : "▶️"}
                      </button>

                      <button
                        onClick={() => removeFromPlaylist(playlist.id, song.id)}
                        className="hover:scale-110 transition-transform"
                      >
                        🗑️
                      </button>
                    </div>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default PlaylistDisplay;