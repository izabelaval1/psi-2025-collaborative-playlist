//handles Spotify search and adding songs to a playlist.

import type { Track} from "../types/Spotify";


// SongService.ts
const BASE_URL = "http://localhost:5000/api";

export const songService = {
  async search(query: string) {
    const res = await fetch(`${BASE_URL}/Spotify/search/${encodeURIComponent(query)}`);
    if (!res.ok) throw new Error("Spotify search failed");
    const data = await res.json();
    return data.tracks?.items || [];
  },

  async addToPlaylist(track: Track, playlistId: number) {
    const songData = {
      PlaylistId: playlistId,
      Title: track.name,
      Album: track.album?.name ?? null,
      DurationMs: track.duration_ms,
      // svarbiausia: siųsti masyvą, o ne vieną string
      ArtistNames: track.artists.map(a => a.name),
      // Url – BE nenaudoja; gali palikti arba išmesti
      // Url: track.external_urls.spotify,
    };

    // Jei projekte aktyvus SongsController (plural), pakeisk į /Songs/add-to-playlist
    const res = await fetch(`${BASE_URL}/song/add-to-playlist`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(songData),
    });

    if (res.status === 409) throw new Error("Song already in playlist");
    if (!res.ok) throw new Error((await res.text()) || "Failed to add song");
  },
};
