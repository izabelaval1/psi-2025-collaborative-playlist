import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Play, Shuffle, Save, Trash2, MoreHorizontal, Clock, Music2, Edit3, Check, X } from "lucide-react";
import { PlaylistService } from "../../../services/PlaylistService";
import type { PlaylistResponseDto } from "../../../types/PlaylistResponseDto";
import "./PlaylistDetailPage.scss";

export default function PlaylistDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [playlist, setPlaylist] = useState<PlaylistResponseDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [sortBy, setSortBy] = useState<"recent" | "title" | "artist">("recent");

  // Edit state
  const [isEditing, setIsEditing] = useState(false);
  const [titleInput, setTitleInput] = useState("");
  const [descriptionInput, setDescriptionInput] = useState("");
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    loadPlaylist();
  }, [id]);

  useEffect(() => {
    if (playlist) {
      setTitleInput(playlist.name);
      setDescriptionInput(playlist.description || "");
    }
  }, [playlist]);

  const loadPlaylist = async () => {
    if (!id) return;

    setIsLoading(true);
    try {
      const data = await PlaylistService.getById(Number(id));
      setPlaylist(data);
    } catch (err) {
      console.error("Failed to load playlist:", err);
      alert("Failed to load playlist");
      navigate("/playlists");
    } finally {
      setIsLoading(false);
    }
  };

  const handleRemoveSong = async (songId: number) => {
    if (!playlist || !confirm("Remove this song from the playlist?")) return;

    try {
      await PlaylistService.removeFromPlaylist(playlist.id, songId);
      await loadPlaylist();
    } catch (err) {
      console.error("Failed to remove song:", err);
      alert("Failed to remove song");
    }
  };

  const getImageUrl = () => {
    if (!playlist?.imageUrl) return `https://picsum.photos/seed/${playlist?.id}/400`;
    if (playlist.imageUrl.startsWith('http')) return playlist.imageUrl;
    return `http://localhost:5000${playlist.imageUrl.startsWith('/') ? '' : '/'}${playlist.imageUrl}`;
  };

  const formatDuration = (seconds?: number) => {
    if (!seconds) return "0:00";
    const minutes = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${minutes}:${secs.toString().padStart(2, '0')}`;
  };

  const getTotalDuration = () => {
    if (!playlist?.songs.length) return "0 min";
    const totalSeconds = playlist.songs.reduce((sum, song) => sum + (song.duration || 0), 0);
    const hours = Math.floor(totalSeconds / 3600);
    const minutes = Math.floor((totalSeconds % 3600) / 60);
    return hours > 0 ? `${hours} hr ${minutes} min` : `${minutes} min`;
  };

  const handleStartEdit = () => {
    if (!playlist) return;
    setTitleInput(playlist.name);
    setDescriptionInput(playlist.description || "");
    setIsEditing(true);
  };

  const handleCancelEdit = () => {
    if (!playlist) return;
    setTitleInput(playlist.name);
    setDescriptionInput(playlist.description || "");
    setIsEditing(false);
  };

  const handleSaveEdit = async () => {
    if (!playlist) return;
    if (!titleInput.trim()) {
      alert("Playlist name can't be empty");
      return;
    }
    setIsSaving(true);
    try {
      const payload = { name: titleInput.trim(), description: descriptionInput.trim() };
      const updated = await PlaylistService.update(playlist.id, payload);
      setPlaylist(updated);
      setIsEditing(false);
    } catch (err) {
      console.error("Failed to update playlist:", err);
      alert("Failed to update playlist");
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return <div className="playlist-detail-page playlist-detail-page--loading">Loading...</div>;
  }

  if (!playlist) {
    return <div className="playlist-detail-page playlist-detail-page--error">Playlist not found</div>;
  }

  return (
    <div className="playlist-detail-page">
      <div className="playlist-detail-page__header">
        <div className="playlist-detail-page__cover">
          <img src={getImageUrl()} alt={playlist.name} />
        </div>

        <div className="playlist-detail-page__info">
          {!isEditing ? (
            <>
              <h1 className="playlist-detail-page__title">{playlist.name}</h1>
              {playlist.description && (
                <p className="playlist-detail-page__description">{playlist.description}</p>
              )}
              <div className="playlist-detail-page__meta">
                <span>Created by <strong>{playlist.host?.username || 'Unknown'}</strong></span>
                <span>•</span>
                <span>{getTotalDuration()}</span>
                <span>•</span>
                <span>{playlist.collaborators?.length || 0} collaborators</span>
              </div>

              <div className="playlist-detail-page__header-actions">
                <button type="button" className="playlist-detail-page__edit-btn" onClick={handleStartEdit}>
                  <Edit3 size={16} /> Edit
                </button>
              </div>
            </>
          ) : (
            <>
              <input
                className="playlist-detail-page__title-input"
                value={titleInput}
                onChange={(e) => setTitleInput(e.target.value)}
              />
              <input
                className="playlist-detail-page__description-input"
                value={descriptionInput}
                onChange={(e) => setDescriptionInput(e.target.value)}
              />
              <div className="playlist-detail-page__edit-actions">
                <button type="button" className="playlist-detail-page__save-btn" onClick={handleSaveEdit} disabled={isSaving}>
                  <Check size={14} /> Save
                </button>
                <button type="button" className="playlist-detail-page__cancel-btn" onClick={handleCancelEdit}>
                  <X size={14} /> Cancel
                </button>
              </div>
            </>
          )}
        </div>
      </div>

      <div className="playlist-detail-page__controls">
        <button type="button" className="playlist-detail-page__btn playlist-detail-page__btn--play">
          <Play size={20} fill="currentColor" />
          Play
        </button>
        <button type="button" className="playlist-detail-page__btn playlist-detail-page__btn--icon">
          <Shuffle size={20} />
          Shuffle
        </button>
        <button type="button" className="playlist-detail-page__btn playlist-detail-page__btn--icon">
          <Save size={20} />
          Save
        </button>
        <button type="button" className="playlist-detail-page__btn playlist-detail-page__btn--icon">
          <Trash2 size={20} />
          Clear queue
        </button>
      </div>

      <div className="playlist-detail-page__sort">
        <span className="playlist-detail-page__sort-label">Sort by</span>
        <button
          type="button"
          className={`playlist-detail-page__sort-btn ${sortBy === 'recent' ? 'playlist-detail-page__sort-btn--active' : ''}`}
          onClick={() => setSortBy('recent')}
        >
          Recently added
        </button>
        <button
          type="button"
          className={`playlist-detail-page__sort-btn ${sortBy === 'title' ? 'playlist-detail-page__sort-btn--active' : ''}`}
          onClick={() => setSortBy('title')}
        >
          By title
        </button>
        <button
          type="button"
          className={`playlist-detail-page__sort-btn ${sortBy === 'artist' ? 'playlist-detail-page__sort-btn--active' : ''}`}
          onClick={() => setSortBy('artist')}
        >
          By artist
        </button>
        <span className="playlist-detail-page__track-count">
          <Music2 size={16} />
          {playlist.songs.length} tracks
        </span>
      </div>

      <div className="playlist-detail-page__tracklist">
        <div className="playlist-detail-page__tracklist-header">
          <span className="playlist-detail-page__col-track">Track</span>
          <span className="playlist-detail-page__col-artist">Artist</span>
          <span className="playlist-detail-page__col-album">Album</span>
          <span className="playlist-detail-page__col-duration">Duration</span>
        </div>

        {playlist.songs.length === 0 ? (
          <div className="playlist-detail-page__empty">
            <Music2 size={48} />
            <p>No tracks yet</p>
            <p className="playlist-detail-page__empty-subtitle">Start adding songs to this playlist</p>
          </div>
        ) : (
          <div className="playlist-detail-page__tracks">
            {playlist.songs.map((song) => (
              <div key={song.id} className="playlist-detail-page__track">
                <div className="playlist-detail-page__track-drag">
                  <Music2 size={16} />
                </div>

                <div className="playlist-detail-page__track-info">
                  <div className="playlist-detail-page__track-name">{song.title}</div>
                </div>

                <div className="playlist-detail-page__track-artist">
                  {song.artists?.map((a) => a.name).join(", ") || "Unknown"}
                </div>

                <div className="playlist-detail-page__track-album">
                  {song.album || "-"}
                </div>

                <div className="playlist-detail-page__track-duration">
                  <Clock size={14} />
                  {song.durationFormatted || formatDuration(song.duration)}
                </div>

                <button
                  type="button"
                  className="playlist-detail-page__track-menu"
                  onClick={() => handleRemoveSong(song.id)}
                  aria-label="Remove song"
                >
                  <MoreHorizontal size={20} />
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}