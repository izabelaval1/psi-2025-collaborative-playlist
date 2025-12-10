import React, { useState, useEffect } from 'react';
import './SpotifyPlayerBar.scss';
import { useSpotifyPlayer } from '../context/SpotifyPlayerContext.tsx';

// Lucide Icons
import { Play, Pause, Volume2 } from 'lucide-react';

const SpotifyPlayerBar: React.FC = () => {
  const { playerState, pause, resume, setVolume } = useSpotifyPlayer();
  const [localPosition, setLocalPosition] = useState(0);

  useEffect(() => {
    setLocalPosition(playerState.position);
  }, [playerState.position]);

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

  if (!playerState.isReady) return null;

  return (
    <div className="playerbar">
      {!playerState.currentTrack ? (
        <div className="playerbar__empty">No track playing</div>
      ) : (
        <div className="playerbar__content">

          {/* LEFT — Track Info */}
          <div className="playerbar__track">
            <div className="playerbar__title">{playerState.currentTrack.title}</div>
            <div className="playerbar__artist">
              {playerState.currentTrack.artists.map(a => a.name).join(', ')}
            </div>
          </div>

          {/* CENTER — Play + Progress */}
          <div className="playerbar__center">
            <button
              onClick={playerState.isPaused ? resume : pause}
              className="playerbar__play"
            >
              {playerState.isPaused ? (
                <Play size={20} />
              ) : (
                <Pause size={20} />
              )}
            </button>

            <div className="playerbar__progress">
              <span className="playerbar__time">{formatTime(localPosition)}</span>

              <input
                type="range"
                min="0"
                max={playerState.duration || 100}
                value={localPosition}
                onChange={handleSeek}
                className="playerbar__seek"
              />

              <span className="playerbar__time">{formatTime(playerState.duration)}</span>
            </div>
          </div>

          {/* RIGHT — Volume */}
          <div className="playerbar__volume">
            <Volume2 className="playerbar__volume-icon" size={20} />

            <input
              type="range"
              min="0"
              max="1"
              step="0.01"
              value={playerState.volume}
              onChange={handleVolumeChange}
              className="playerbar__volume-slider"
            />
          </div>
        </div>
      )}
    </div>
  );
};

export default SpotifyPlayerBar;
