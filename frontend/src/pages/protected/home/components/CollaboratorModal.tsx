import { useState, useEffect } from "react";
import { UserPlus, AlertCircle, Search } from "lucide-react";
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

  // ✅ Live search - triggers as user types (with debounce)
  useEffect(() => {
    if (username.trim().length < 2) {
      setSearchResults([]);
      setIsSearching(false);
      return;
    }

    setIsSearching(true);
    const timeoutId = setTimeout(async () => {
      try {
        const results = await PlaylistService.searchUsers(username);
        setSearchResults(results);
      } catch (err) {
        console.error("Failed to search users:", err);
        setSearchResults([]);
      } finally {
        setIsSearching(false);
      }
    }, 300); // 300ms debounce

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
      
      let errorMessage = "Failed to add collaborator.";
      
      if (err instanceof Error) {
        try {
          const errorData = JSON.parse(err.message);
          errorMessage = errorData.message || errorMessage;
        } catch {
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
          Start typing to search for users. Click on a result to add them instantly.
        </p>

        {error && (
          <div className="collaborator-modal__error">
            <AlertCircle size={18} />
            <span>{error}</span>
          </div>
        )}

        <div className="collaborator-modal__search-wrapper">
          <div className="collaborator-modal__input-container">
            <Search size={18} className="collaborator-modal__search-icon" />
            <input
              type="text"
              placeholder="Type username..."
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              onKeyDown={handleKeyDown}
              className="collaborator-modal__input"
              autoFocus
            />
          </div>

          {/* Live search results dropdown */}
          {username.trim().length >= 2 && (
            <div className="collaborator-modal__results">
              {isSearching ? (
                <div className="collaborator-modal__searching">
                  <div className="collaborator-modal__spinner"></div>
                  <span>Searching users...</span>
                </div>
              ) : searchResults.length > 0 ? (
                <>
                  {searchResults.map((user) => (
                    <button
                      key={user.id}
                      type="button"
                      onClick={() => handleAddCollaborator(user.username)}
                      className="collaborator-modal__result-item"
                      disabled={isAdding}
                    >
                      <div className="collaborator-modal__result-avatar">
                        {user.username.charAt(0).toUpperCase()}
                      </div>
                      <div className="collaborator-modal__result-info">
                        <div className="collaborator-modal__result-username">
                          {user.username}
                        </div>
                        <div className="collaborator-modal__result-hint">
                          Click to add as collaborator
                        </div>
                      </div>
                      <div className="collaborator-modal__add-icon">+</div>
                    </button>
                  ))}
                </>
              ) : (
                <div className="collaborator-modal__no-results">
                  <Search size={24} />
                  <p>No users found matching "{username}"</p>
                  <p className="collaborator-modal__no-results-hint">
                    Try a different search term
                  </p>
                </div>
              )}
            </div>
          )}

          {username.trim().length > 0 && username.trim().length < 2 && (
            <div className="collaborator-modal__hint">
              Type at least 2 characters to search
            </div>
          )}
        </div>

        <div className="collaborator-modal__actions">
          <button
            type="button"
            onClick={onClose}
            className="collaborator-modal__btn collaborator-modal__btn--secondary"
            disabled={isAdding}
          >
            Cancel
          </button>
          <button
            type="button"
            onClick={() => handleAddCollaborator()}
            disabled={isAdding || !username.trim() || username.trim().length < 2}
            className="collaborator-modal__btn collaborator-modal__btn--primary"
          >
            {isAdding ? (
              <>
                <div className="collaborator-modal__spinner collaborator-modal__spinner--small"></div>
                Adding...
              </>
            ) : (
              "Add Collaborator"
            )}
          </button>
        </div>
      </div>
    </div>
  );
}