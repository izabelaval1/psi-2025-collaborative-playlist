ALTER TABLE playlists
ALTER COLUMN host_id TYPE integer USING host_id::integer;
