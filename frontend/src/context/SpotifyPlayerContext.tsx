import React, { createContext, useContext, useState, useEffect, useCallback } from "react";
import type { Song } from "../types/Song";

interface PlayerState {
  isPlaying: boolean;
  currentTrack: Song | null;
  position: number;
  duration: number;
  volume: number;
  isPaused: boolean;
  isReady: boolean;
  currentTrackUri: string | null;
}

interface SpotifyPlayerContextType {
  playerState: PlayerState;
  spotifyToken: string | null;
  deviceId: string | null;

  play: (trackUri: string, type?: "track" | "playlist") => Promise<void>;
  stop: () => Promise<void>;
  pause: () => Promise<void>;
  resume: () => Promise<void>;
  setVolume: (volume: number) => Promise<void>;
  setSpotifyToken: (token: string) => void;
}

const SpotifyPlayerContext = createContext<SpotifyPlayerContextType | undefined>(undefined);

export const SpotifyPlayerProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [playerState, setPlayerState] = useState<PlayerState>({
    isPlaying: false,
    currentTrack: null,
    position: 0,
    duration: 0,
    volume: 0.5,
    isPaused: true,
    isReady: false,
    currentTrackUri: null,
  });

  const [spotifyToken, setSpotifyTokenState] = useState<string | null>(null);
  const [deviceId, setDeviceId] = useState<string | null>(null);
  const [player, setPlayer] = useState<any>(null);


  
  const setSpotifyToken = useCallback((token: string) => {
    setSpotifyTokenState(token);
  }, []);

  // Initialize Spotify Web Playback SDK
  useEffect(() => {
    if (!spotifyToken) return;

    const initializePlayer = () => {
      const spotifyPlayer = new (window as any).Spotify.Player({
        name: "Playlist Web Player",
        getOAuthToken: (cb: (token: string) => void) => cb(spotifyToken),
        volume: 0.5,
      });

      spotifyPlayer.addListener("ready", ({ device_id }: any) => {
        setDeviceId(device_id);
        setPlayerState(prev => ({ ...prev, isReady: true }));
      });

      spotifyPlayer.addListener("player_state_changed", (state: any) => {
        if (!state) return;

        const track = state.track_window.current_track;

        setPlayerState(prev => ({
          ...prev,
          isPlaying: !state.paused,
          isPaused: state.paused,
          position: Math.floor(state.position / 1000),
          duration: Math.floor(state.duration / 1000),
          currentTrackUri: track ? track.uri : null,
          currentTrack: track
            ? {
                id: 0,
                spotifyId: track.id,
                spotifyUri: track.uri,
                title: track.name,
                artists: track.artists.map((a: any) => ({ name: a.name })),
                album: track.album?.name || "",
                duration: Math.floor(state.duration / 1000),
                durationFormatted: "",
              }
            : null,
        }));
      });

      spotifyPlayer.connect();
      setPlayer(spotifyPlayer);
    };

    if ((window as any).Spotify) {
      initializePlayer();
      return;
    }

    const script = document.createElement("script");
    script.src = "https://sdk.scdn.co/spotify-player.js";
    script.async = true;
    document.body.appendChild(script);

    (window as any).onSpotifyWebPlaybackSDKReady = initializePlayer;

    return () => {
      if (player) player.disconnect();
    };
  }, [spotifyToken]);

  // PLAY
  const play = useCallback(
    async (uri: string, type: "track" | "playlist" = "track") => {
      if (!spotifyToken || !deviceId) return;

      let body: any = {};

      if (type === "track") {
        body = { uris: [uri] };
      } else if (type === "playlist") {
        body = { context_uri: uri };
      }

      await fetch(`https://api.spotify.com/v1/me/player/play?device_id=${deviceId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${spotifyToken}`,
        },
        body: JSON.stringify(body),
      });

      setPlayerState(prev => ({
        ...prev,
        currentTrackUri: uri,
      }));
    },
    [spotifyToken, deviceId]
  );

  // STOP — forces pause + resets state
  const stop = useCallback(async () => {
    if (!spotifyToken || !deviceId) return;

    await fetch(`https://api.spotify.com/v1/me/player/pause?device_id=${deviceId}`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${spotifyToken}` },
    });

    setPlayerState(prev => ({
      ...prev,
      isPlaying: false,
      isPaused: true,
      position: 0,
    }));
  }, [spotifyToken, deviceId]);

  // PAUSE
  const pause = useCallback(async () => {
    if (!player) return;
    await player.pause();
  }, [player]);

  // RESUME
  const resume = useCallback(async () => {
    if (!player) return;
    await player.resume();
  }, [player]);

  // VOLUME
  const setVolume = useCallback(
    async (volume: number) => {
      if (!player) return;
      await player.setVolume(volume);
      setPlayerState(prev => ({ ...prev, volume }));
    },
    [player]
  );

  const value: SpotifyPlayerContextType = {
    playerState,
    spotifyToken,
    deviceId,
    play,
    stop,
    pause,
    resume,
    setVolume,
    setSpotifyToken,
  };

  return <SpotifyPlayerContext.Provider value={value}>{children}</SpotifyPlayerContext.Provider>;
};

export const useSpotifyPlayer = () => {
  const context = useContext(SpotifyPlayerContext);
  if (!context) throw new Error("useSpotifyPlayer must be used within SpotifyPlayerProvider");
  return context;
};
