import { useState } from "react";
import { Radio, Users, Clock, Music2, Plus, Sparkles, TrendingUp } from "lucide-react";
import { authService } from "../../../services/authService";
import Modal from "../../../components/Modal";
import "./LiveSessionsPage.scss";

interface LiveSession {
  id: number;
  name: string;
  hostUsername: string;
  hostId: number;
  activeUsers: number;
  currentSong?: string;
  currentArtist?: string;
  startedAt: Date;
  isPublic: boolean;
}

export default function LiveSessionsPage() {
  const [sessions, setSessions] = useState<LiveSession[]>([
    {
      id: 1,
      name: "Friday Night Vibes",
      hostUsername: "DJ_Sarah",
      hostId: 1,
      activeUsers: 8,
      currentSong: "Blinding Lights",
      currentArtist: "The Weeknd",
      startedAt: new Date(Date.now() - 1800000),
      isPublic: true,
    },
    {
      id: 2,
      name: "Chill Study Session",
      hostUsername: "StudyBuddy",
      hostId: 2,
      activeUsers: 5,
      currentSong: "Lofi Hip Hop Beat",
      currentArtist: "Chillhop Music",
      startedAt: new Date(Date.now() - 3600000),
      isPublic: true,
    },
    {
      id: 3,
      name: "Rock Classics",
      hostUsername: "RockFan92",
      hostId: 3,
      activeUsers: 12,
      currentSong: "Bohemian Rhapsody",
      currentArtist: "Queen",
      startedAt: new Date(Date.now() - 900000),
      isPublic: true,
    },
  ]);

  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [newSessionName, setNewSessionName] = useState("");
  const [isPublic, setIsPublic] = useState(true);
  const [isCreating, setIsCreating] = useState(false);

  const currentUser = authService.getUser();

  const getTimeAgo = (date: Date) => {
    const minutes = Math.floor((Date.now() - date.getTime()) / 60000);
    if (minutes < 1) return "Just now";
    if (minutes < 60) return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    return `${hours}h ago`;
  };

  const handleCreateSession = () => {
    if (!newSessionName.trim() || !currentUser) return;

    setIsCreating(true);

    setTimeout(() => {
      const newSession: LiveSession = {
        id: Date.now(),
        name: newSessionName.trim(),
        hostUsername: currentUser.username,
        hostId: currentUser.id,
        activeUsers: 1,
        startedAt: new Date(),
        isPublic,
      };

      setSessions([newSession, ...sessions]);
      setNewSessionName("");
      setIsPublic(true);
      setIsCreateModalOpen(false);
      setIsCreating(false);

      // In real app, navigate to the session room
      alert(`Session "${newSession.name}" created! (Will navigate to session room)`);
    }, 500);
  };

  const handleJoinSession = (session: LiveSession) => {
    // In real app, navigate to session room with WebSocket connection
    alert(`Joining "${session.name}"... (Will implement WebSocket connection)`);
  };

  const mostActiveSession = sessions.reduce((prev, current) => 
    (current.activeUsers > prev.activeUsers) ? current : prev
  , sessions[0]);

  return (
    <div className="live-sessions-page">
      <div className="live-sessions-page__header">
        <div className="live-sessions-page__header-content">
          <div className="live-sessions-page__icon-wrapper">
            <Radio size={32} className="live-sessions-page__icon" />
            <span className="live-sessions-page__live-badge">LIVE</span>
          </div>
          <div>
            <h1 className="live-sessions-page__title">Live Sessions</h1>
            <p className="live-sessions-page__subtitle">
              {sessions.length} active listening room{sessions.length !== 1 ? 's' : ''} • Join or create your own
            </p>
          </div>
        </div>
        <button
          type="button"
          className="live-sessions-page__create-btn"
          onClick={() => setIsCreateModalOpen(true)}
        >
          <Plus size={20} />
          Start Session
        </button>
      </div>

      <div className="live-sessions-page__info-banner">
        <Sparkles size={20} />
        <div>
          <strong>What are Live Sessions?</strong>
          <p>Temporary listening rooms where you can play music in real-time with friends. No playlist needed - just start a session and share songs on the fly!</p>
        </div>
      </div>

      {mostActiveSession && (
        <div className="live-sessions-page__featured">
          <div className="live-sessions-page__featured-badge">
            <TrendingUp size={16} />
            <span>Most Popular Right Now</span>
          </div>
          <div className="live-sessions-page__featured-card">
            <div className="live-sessions-page__featured-visual">
              <div className="live-sessions-page__featured-avatar">
                {mostActiveSession.hostUsername.charAt(0).toUpperCase()}
              </div>
              <div className="live-sessions-page__featured-pulse-rings">
                <span className="live-sessions-page__pulse-ring"></span>
                <span className="live-sessions-page__pulse-ring"></span>
                <span className="live-sessions-page__pulse-ring"></span>
              </div>
            </div>
            <div className="live-sessions-page__featured-info">
              <h2 className="live-sessions-page__featured-title">
                {mostActiveSession.name}
              </h2>
              <p className="live-sessions-page__featured-host">
                Hosted by {mostActiveSession.hostUsername}
              </p>
              {mostActiveSession.currentSong && (
                <div className="live-sessions-page__featured-current">
                  <Music2 size={16} />
                  <span>
                    Now playing: {mostActiveSession.currentSong} • {mostActiveSession.currentArtist}
                  </span>
                </div>
              )}
              <div className="live-sessions-page__featured-meta">
                <div className="live-sessions-page__featured-users">
                  <Users size={18} />
                  <span>{mostActiveSession.activeUsers} listening</span>
                </div>
                <div className="live-sessions-page__featured-time">
                  <Clock size={18} />
                  <span>{getTimeAgo(mostActiveSession.startedAt)}</span>
                </div>
              </div>
              <button
                type="button"
                className="live-sessions-page__featured-join"
                onClick={() => handleJoinSession(mostActiveSession)}
              >
                Join Session
              </button>
            </div>
          </div>
        </div>
      )}

      <div className="live-sessions-page__list">
        <h2 className="live-sessions-page__list-title">All Active Sessions</h2>
        <div className="live-sessions-page__grid">
          {sessions.map((session) => (
            <div
              key={session.id}
              className="live-sessions-page__card"
              onClick={() => handleJoinSession(session)}
            >
              <div className="live-sessions-page__card-header">
                <div className="live-sessions-page__card-avatar">
                  {session.hostUsername.charAt(0).toUpperCase()}
                </div>
                <div className="live-sessions-page__card-live-indicator">
                  <span className="live-sessions-page__pulse"></span>
                  LIVE
                </div>
              </div>
              <div className="live-sessions-page__card-content">
                <h3 className="live-sessions-page__card-title">
                  {session.name}
                </h3>
                <p className="live-sessions-page__card-host">
                  by {session.hostUsername}
                </p>
                {session.currentSong && (
                  <div className="live-sessions-page__card-current">
                    <Music2 size={14} />
                    <span>
                      {session.currentSong.length > 25
                        ? session.currentSong.substring(0, 25) + "..."
                        : session.currentSong}
                    </span>
                  </div>
                )}
                <div className="live-sessions-page__card-footer">
                  <div className="live-sessions-page__card-users">
                    <Users size={14} />
                    <span>{session.activeUsers} listening</span>
                  </div>
                  <span className="live-sessions-page__card-time">
                    {getTimeAgo(session.startedAt)}
                  </span>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Create Session Modal */}
      <Modal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        title="Start a Live Session"
      >
        <div className="live-sessions-page__create-modal">
          <p className="live-sessions-page__create-description">
            Create a temporary listening room. You can add songs on the fly and listen together in real-time.
          </p>

          <div className="live-sessions-page__create-field">
            <label className="live-sessions-page__create-label">
              Session Name
              <span className="live-sessions-page__required">*</span>
            </label>
            <input
              type="text"
              placeholder="Friday Night Vibes, Study Together, etc."
              value={newSessionName}
              onChange={(e) => setNewSessionName(e.target.value)}
              className="live-sessions-page__create-input"
              maxLength={50}
              autoFocus
            />
          </div>

          <div className="live-sessions-page__create-visibility">
            <label className="live-sessions-page__create-label">Visibility</label>
            <div className="live-sessions-page__toggle-group">
              <button
                type="button"
                className={`live-sessions-page__toggle ${isPublic ? 'live-sessions-page__toggle--active' : ''}`}
                onClick={() => setIsPublic(true)}
              >
                Public
                <span className="live-sessions-page__toggle-hint">Anyone can join</span>
              </button>
              <button
                type="button"
                className={`live-sessions-page__toggle ${!isPublic ? 'live-sessions-page__toggle--active' : ''}`}
                onClick={() => setIsPublic(false)}
              >
                Private
                <span className="live-sessions-page__toggle-hint">Link only</span>
              </button>
            </div>
          </div>

          <div className="live-sessions-page__create-actions">
            <button
              type="button"
              onClick={() => setIsCreateModalOpen(false)}
              className="live-sessions-page__create-btn-secondary"
              disabled={isCreating}
            >
              Cancel
            </button>
            <button
              type="button"
              onClick={handleCreateSession}
              disabled={!newSessionName.trim() || isCreating}
              className="live-sessions-page__create-btn-primary"
            >
              {isCreating ? "Creating..." : "Start Session"}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
}