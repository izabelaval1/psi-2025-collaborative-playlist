import type { Track } from "../types/Spotify";
import api from "./api";

export const songService = {
  async search(query: string) {
    const res = await api.get(`/api/Spotify/search/${encodeURIComponent(query)}`);
    return res.data?.tracks?.items || [];
  },

  async addToPlaylist(track: Track, playlistId: number) {
    const currentUser = JSON.parse(localStorage.getItem("user") || "null");
    
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

    try {
      await api.post(`/api/song/add-to-playlist`, songData);
    } catch (err: any) {
      const status = err?.response?.status;
      const msg = err?.response?.data?.message || err?.response?.data || err?.message;

      if (status === 409) throw new Error("Song already in playlist");
      throw new Error(msg || "Failed to add song");
    }
  },
};