import "./AddPlaylistCard.scss";

export default function AddPlaylistCard() {
    return (
        <div className="add-playlist-card">
            <h3 className="add-playlist-card__title">Start a new collaborative playlist</h3>
            <p className="add-playlist-card__description">Create a shared playlist and invite friends.</p>
            <button type="button" className="add-playlist-card__button">New playlist</button>
        </div>
    )
}