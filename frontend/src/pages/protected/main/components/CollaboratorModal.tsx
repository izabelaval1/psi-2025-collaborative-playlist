import { useState, useEffect } from "react";
import { UserPlus, AlertCircle } from "lucide-react";
import { PlaylistService } from "../../../../services/PlaylistService";
import "./CollaboratorModal.scss";

interface CollaboratorModalProps {
  playlistId: number;
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

interface UserSearchResult {
  id: number;
  username: string;
}

export default function CollaboratorModal({ 
  playlistId, 
  isOpen, 
  onClose, 
  onSuccess 
}: CollaboratorModalProps) {
  const [username, setUsername] = useState("");
  const [searchResults, setSearchResults] = useState<UserSearchResult[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [isAdding, setIsAdding] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Search users as they type
  useEffect(() => {
    if (username.trim().length < 2) {
      setSearchResults([]);
      return;
    }

    const timeoutId = setTimeout(async () => {
      setIsSearching(true);
      try {
        const results = await PlaylistService.searchUsers(username);
        setSearchResults(results);
      } catch (err) {
        console.error("Failed to search users:", err);
        setSearchResults([]);
      } finally {
        setIsSearching(false);
      }
    }, 300); // Debounce 300ms

    return () => clearTimeout(timeoutId);
  }, [username]);

  // Clear error when user starts typing
  useEffect(() => {
    if (error) setError(null);
  }, [username]);

  const handleAddCollaborator = async (selectedUsername?: string) => {
    const usernameToAdd = selectedUsername || username.trim();
    
    if (!usernameToAdd) return;

    setIsAdding(true);
    setError(null);
    
    try {
      await PlaylistService.addCollaborator(playlistId, usernameToAdd);
      setUsername("");
      setSearchResults([]);
      onSuccess();
      onClose();
    } catch (err) {
      console.error("Failed to add collaborator:", err);
      
      // Extract clean error message
      let errorMessage = "Failed to add collaborator.";
      
      if (err instanceof Error) {
        // Try to parse JSON error response
        try {
          const errorData = JSON.parse(err.message);
          errorMessage = errorData.message || errorMessage;
        } catch {
          // If not JSON, use the error message as is
          errorMessage = err.message;
        }
      }
      
      setError(errorMessage);
    } finally {
      setIsAdding(false);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && username.trim() && !isAdding) {
      handleAddCollaborator();
    }
  };

  if (!isOpen) return null;

  return (
    <div className="collaborator-modal__overlay" onClick={onClose}>
      <div className="collaborator-modal" onClick={(e) => e.stopPropagation()}>
        <div className="collaborator-modal__header">
          <UserPlus size={24} />
          <h3 className="collaborator-modal__title">Add Collaborator</h3>
        </div>

        <p className="collaborator-modal__description">
          Search for a user to invite as a collaborator
        </p>

        {error && (
          <div className="collaborator-modal__error">
            <AlertCircle size={18} />
            <span>{error}</span>
          </div>
        )}

        <div className="collaborator-modal__search-wrapper">
          <input
            type="text"
            placeholder="Type username..."
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            onKeyDown={handleKeyDown}
            className="collaborator-modal__input"
            autoFocus
          />

          {/* Search results dropdown */}
          {searchResults.length > 0 && (
            <div className="collaborator-modal__results">
              {searchResults.map((user) => (
                <button
                  key={user.id}
                  type="button"
                  onClick={() => handleAddCollaborator(user.username)}
                  className="collaborator-modal__result-item"
                >
                  <div className="collaborator-modal__result-username">
                    {user.username}
                  </div>
                  <div className="collaborator-modal__result-hint">
                    Click to add
                  </div>
                </button>
              ))}
            </div>
          )}

          {isSearching && (
            <div className="collaborator-modal__searching">
              Searching...
            </div>
          )}

          {username.trim().length >= 2 && !isSearching && searchResults.length === 0 && (
            <div className="collaborator-modal__no-results">
              No users found matching "{username}"
            </div>
          )}
        </div>

        <div className="collaborator-modal__actions">
          <button
            type="button"
            onClick={onClose}
            className="collaborator-modal__btn collaborator-modal__btn--secondary"
          >
            Cancel
          </button>
          <button
            type="button"
            onClick={() => handleAddCollaborator()}
            disabled={isAdding || !username.trim()}
            className="collaborator-modal__btn collaborator-modal__btn--primary"
          >
            {isAdding ? "Adding..." : "Add Collaborator"}
          </button>
        </div>
      </div>
    </div>
  );
}