// Bottom player bar showing current track and controls
import React, { useState, useEffect } from 'react';
import { useSpotifyPlayer } from '../context/SpotifyPlayerContext.tsx';

const SpotifyPlayerBar: React.FC = () => {
  const { playerState, pause, resume, setVolume } = useSpotifyPlayer();
  const [localPosition, setLocalPosition] = useState(0);

  // Update position locally for smooth progress bar
  useEffect(() => {
    setLocalPosition(playerState.position);
  }, [playerState.position]);

  // Update position every second while playing
  useEffect(() => {
    if (!playerState.isPlaying) return;

    const interval = setInterval(() => {
      setLocalPosition(prev => Math.min(prev + 1, playerState.duration));
    }, 1000);

    return () => clearInterval(interval);
  }, [playerState.isPlaying, playerState.duration]);

  const handleSeek = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newPosition = parseFloat(e.target.value);
    setLocalPosition(newPosition);
    //seek(newPosition);
  };

  const handleVolumeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newVolume = parseFloat(e.target.value);
    setVolume(newVolume);
  };

  const formatTime = (seconds: number): string => {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  };

  if ( !playerState.isReady) {
    return null; // Don't show player if not connected
  }

  if (!playerState.currentTrack) {
    return (
      <div className="fixed bottom-0 left-0 right-0 bg-neutral-900 border-t border-neutral-700 p-3">
        <div className="flex items-center justify-center text-gray-400 text-sm">
          No track playing
        </div>
      </div>
    );
  }

  return (
    <div className="fixed bottom-0 left-0 right-0 bg-neutral-900 border-t border-neutral-700 p-4 shadow-lg">
      <div className="max-w-screen-xl mx-auto">
        <div className="flex items-center gap-4">
          {/* Track info */}
          <div className="flex-1 min-w-0">
            <div className="text-white font-medium truncate">
              {playerState.currentTrack.title}
            </div>
            <div className="text-gray-400 text-sm truncate">
              {playerState.currentTrack.artists.map(a => a.name).join(', ')}
            </div>
          </div>

          {/* Playback controls */}
          <div className="flex flex-col items-center gap-2 flex-1">
            <div className="flex items-center gap-4">
              <button
                onClick={playerState.isPaused ? resume : pause}
                className="bg-white text-black rounded-full p-2 hover:scale-105 transition-transform"
              >
                {playerState.isPaused ? '▶️' : '⏸️'}
              </button>
            </div>

            {/* Progress bar */}
            <div className="flex items-center gap-2 w-full">
              <span className="text-xs text-gray-400 w-10 text-right">
                {formatTime(localPosition)}
              </span>
              <input
                type="range"
                min="0"
                max={playerState.duration || 100}
                value={localPosition}
                onChange={handleSeek}
                className="flex-1 h-1 bg-neutral-700 rounded-lg appearance-none cursor-pointer"
                style={{
                  background: `linear-gradient(to right, #1db954 0%, #1db954 ${(localPosition / playerState.duration) * 100}%, #404040 ${(localPosition / playerState.duration) * 100}%, #404040 100%)`
                }}
              />
              <span className="text-xs text-gray-400 w-10">
                {formatTime(playerState.duration)}
              </span>
            </div>
          </div>

          {/* Volume control */}
          <div className="flex items-center gap-2 flex-1 justify-end">
            <span className="text-gray-400">🔊</span>
            <input
              type="range"
              min="0"
              max="1"
              step="0.01"
              value={playerState.volume}
              onChange={handleVolumeChange}
              className="w-24 h-1 bg-neutral-700 rounded-lg appearance-none cursor-pointer"
              style={{
                background: `linear-gradient(to right, #1db954 0%, #1db954 ${playerState.volume * 100}%, #404040 ${playerState.volume * 100}%, #404040 100%)`
              }}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default SpotifyPlayerBar;