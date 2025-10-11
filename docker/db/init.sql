-- Create tables
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    password_hash VARCHAR(255) NOT NULL
);

CREATE TABLE playlists (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    host_id INTEGER REFERENCES users(id)
);

CREATE TABLE songs (
    id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    album VARCHAR(100),
    duration INTEGER
);

CREATE TABLE artists (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE song_artists (
    song_id INTEGER REFERENCES songs(id),
    artist_id INTEGER REFERENCES artists(id),
    PRIMARY KEY (song_id, artist_id)
);

CREATE TABLE playlist_songs (
    playlist_id INTEGER REFERENCES playlists(id),
    song_id INTEGER REFERENCES songs(id),
    position INTEGER,
    PRIMARY KEY (playlist_id, song_id)
);

CREATE TABLE playlist_collaborators (
    playlist_id INTEGER REFERENCES playlists(id),
    user_id INTEGER REFERENCES users(id),
    PRIMARY KEY (playlist_id, user_id)
);

-- Insert mock data for playlists
INSERT INTO playlists (id, name, description, host_id) VALUES
(1, 'Focus Beats', 'Music to help you concentrate', NULL),
(2, 'Workout Pump', 'Energetic songs for the gym', NULL),
(3, 'Chill Vibes', 'Relax and unwind with these tracks', NULL),
(4, 'Party Hits', 'Upbeat songs to keep the party going', NULL);

-- Insert artists
INSERT INTO artists (name) VALUES
('Bad Bunny'),
('ABBA'),
('Chico, Qatoshi'),
('Ricchi E Poveri');

-- Insert songs
INSERT INTO songs (title, album) VALUES
('Efecto', 'Un Verano Sin Ti'),
('Mamma Mia', 'Abba'),
('Mamma Mia', 'Mamma Mia'),
('Mamma Maria', 'The Collection');

-- Link songs to artists
INSERT INTO song_artists (song_id, artist_id) VALUES
(1, 1), -- Efecto by Bad Bunny
(2, 2), -- Mamma Mia by ABBA
(3, 3), -- Mamma Mia by Chico, Qatoshi
(4, 4); -- Mamma Maria by Ricchi E Poveri

-- Add songs to playlists
INSERT INTO playlist_songs (playlist_id, song_id, position) VALUES
(3, 1, 1), -- Efecto in Chill Vibes
(4, 2, 1), -- Mamma Mia (ABBA) in Party Hits
(4, 3, 2), -- Mamma Mia (Chico, Qatoshi) in Party Hits
(4, 4, 3); -- Mamma Maria in Party Hits

-- Reset sequences to continue from correct values
SELECT setval('playlists_id_seq', (SELECT MAX(id) FROM playlists));
SELECT setval('artists_id_seq', (SELECT MAX(id) FROM artists));
SELECT setval('songs_id_seq', (SELECT MAX(id) FROM songs));