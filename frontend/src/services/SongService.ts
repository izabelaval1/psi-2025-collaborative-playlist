import type { Track } from "../types/Spotify";
import { authService } from "./authService";

const BASE_URL = "http://localhost:5000/api";

export const songService = {
  async search(query: string) {
    const res = await fetch(`${BASE_URL}/Spotify/search/${encodeURIComponent(query)}`);
    if (!res.ok) throw new Error("Spotify search failed");
    const data = await res.json();
    return data.tracks?.items || [];
  },

  async addToPlaylist(track: Track, playlistId: number) {
    const currentUser = authService.getUser();
    
    const songData = {
      PlaylistId: playlistId,
      Title: track.name,
      Album: track.album?.name ?? null,
      DurationMs: track.duration_ms,
      ArtistNames: track.artists.map(a => a.name),
      
      //track who added the song
      AddedByUserId: currentUser?.id,
      
      //Spotify integration
      SpotifyId: track.id,
      SpotifyUri: track.uri,
    };

    const res = await fetch(`${BASE_URL}/song/add-to-playlist`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(songData),
    });

    if (res.status === 409) throw new Error("Song already in playlist");
    if (!res.ok) throw new Error((await res.text()) || "Failed to add song");
  },
};