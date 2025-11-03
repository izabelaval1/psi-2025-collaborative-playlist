//handles Spotify search and adding songs to a playlist.

import type { Track, SpotifyResponse } from "../types/Spotify";

const BASE_URL = "http://localhost:5000/api";

export const songService = {
  async search(query: string): Promise<Track[]> {
    const res = await fetch(`${BASE_URL}/Spotify/search/${encodeURIComponent(query)}`);
    if (!res.ok) throw new Error("Spotify search failed");
    const data: SpotifyResponse = await res.json();
    return data.tracks?.items || [];
  },

  async addToPlaylist(track: Track, playlistId: number) {
    const songData = {
      PlaylistId: playlistId,
      Title: track.name,
      Artist: track.artists.map((a) => a.name).join(", "),
      Album: track.album.name,
      Url: track.external_urls.spotify,
    };
    const res = await fetch(`${BASE_URL}/songs/add-to-playlist`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(songData),
    });
    if (res.status === 409) throw new Error("Song already in playlist");
    if (!res.ok) throw new Error(await res.text() || "Failed to add song");
  },
};
