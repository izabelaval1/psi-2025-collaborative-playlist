import { useState } from 'react';

// Type definitions
interface Artist {
  id: number;
  name: string;
}

interface UserDto {
  id: number;
  username: string;
  role?: string;
  profileImage?: string;
}

interface SongDto {
  id: number;
  title: string;
  album?: string;
  duration?: number;
  durationFormatted?: string;
  position?: number;
  artists: Artist[];
  addedBy?: UserDto;
  addedAt?: string;
}

interface PlaylistResponse {
  id: number;
  name: string;
  description?: string;
  songs: SongDto[];
  collaborators: UserDto[];
  imageUrl?: string;
}

export default function DebugAddedBy() {
  const [playlistId, setPlaylistId] = useState('');
  const [response, setResponse] = useState<PlaylistResponse | null>(null);
  const [error, setError] = useState('');

  const fetchPlaylist = async () => {
    try {
      setError('');
      const res = await fetch(`http://localhost:5000/api/playlists/${playlistId}`);
      if (!res.ok) throw new Error('Failed to fetch');
      const data = await res.json();
      setResponse(data);
      console.log('Full API Response:', data);
    } catch (err) {
      setError((err as Error).message);
    }
  };

  return (
    <div className="p-8 max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold mb-4">Debug AddedBy Issue</h1>
      
      <div className="mb-6">
        <label className="block mb-2 font-semibold">Enter Playlist ID:</label>
        <div className="flex gap-2">
          <input
            type="number"
            value={playlistId}
            onChange={(e) => setPlaylistId(e.target.value)}
            className="border rounded px-3 py-2 flex-1"
            placeholder="e.g., 1"
          />
          <button
            onClick={fetchPlaylist}
            className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
          >
            Fetch Playlist
          </button>
        </div>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      {response && (
        <div className="space-y-4">
          <div className="bg-green-100 border border-green-400 px-4 py-3 rounded">
            <h2 className="font-bold mb-2">✓ Playlist fetched successfully</h2>
            <p>Name: {response.name}</p>
            <p>Songs count: {response.songs?.length || 0}</p>
          </div>

          <div className="bg-gray-100 p-4 rounded">
            <h3 className="font-bold mb-2">Songs in playlist:</h3>
            {response.songs?.length > 0 ? (
              <div className="space-y-3">
                {response.songs.map((song, idx) => (
                  <div key={idx} className="border-l-4 border-blue-500 pl-3 py-2 bg-white">
                    <div className="font-semibold">{song.title}</div>
                    <div className="text-sm text-gray-600">
                      Artists: {song.artists?.map(a => a.name).join(', ') || 'Unknown'}
                    </div>
                    <div className="text-sm mt-2">
                      <span className="font-semibold">AddedBy object: </span>
                      {song.addedBy ? (
                        <span className="text-green-600">
                          ✓ EXISTS - Username: {song.addedBy.username} (ID: {song.addedBy.id})
                        </span>
                      ) : (
                        <span className="text-red-600">✗ NULL or UNDEFINED</span>
                      )}
                    </div>
                    <div className="text-sm">
                      <span className="font-semibold">AddedAt: </span>
                      {song.addedAt || 'null'}
                    </div>
                    <details className="mt-2">
                      <summary className="cursor-pointer text-sm text-blue-600">
                        View raw song object
                      </summary>
                      <pre className="text-xs bg-gray-50 p-2 rounded mt-1 overflow-auto">
                        {JSON.stringify(song, null, 2)}
                      </pre>
                    </details>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-500">No songs in playlist</p>
            )}
          </div>

          <details className="bg-gray-100 p-4 rounded">
            <summary className="cursor-pointer font-bold">View Full API Response</summary>
            <pre className="text-xs bg-white p-3 rounded mt-2 overflow-auto max-h-96">
              {JSON.stringify(response, null, 2)}
            </pre>
          </details>

          <div className="bg-yellow-100 border border-yellow-400 px-4 py-3 rounded">
            <h3 className="font-bold mb-2">Debugging Steps:</h3>
            <ol className="list-decimal list-inside space-y-1 text-sm">
              <li>Check if <code className="bg-white px-1">addedBy</code> exists in the raw JSON above</li>
              <li>If it's null, check your database: <code className="bg-white px-1">SELECT * FROM playlist_songs WHERE playlist_id = {playlistId}</code></li>
              <li>Verify <code className="bg-white px-1">added_by_user_id</code> column has values in DB</li>
              <li>Check backend logs when fetching playlist</li>
              <li>Ensure <code className="bg-white px-1">Include(p =&gt; p.PlaylistSongs).ThenInclude(ps =&gt; ps.AddedBy)</code> is being called</li>
            </ol>
          </div>
        </div>
      )}

      <div className="mt-8 p-4 bg-blue-50 rounded">
        <h3 className="font-bold mb-2">Quick Database Check:</h3>
        <p className="text-sm mb-2">Run this SQL query to check your data:</p>
        <pre className="bg-white p-3 rounded text-xs overflow-auto">
{`SELECT 
  p.name as playlist_name,
  s.title as song_title,
  ps.added_by_user_id,
  ps.added_at,
  u.username as added_by_username
FROM playlist_songs ps
JOIN playlists p ON ps.playlist_id = p.id
JOIN songs s ON ps.song_id = s.id
LEFT JOIN users u ON ps.added_by_user_id = u.id
WHERE p.id = ${playlistId || '[YOUR_PLAYLIST_ID]'}
ORDER BY ps.position;`}
        </pre>
      </div>
    </div>
  );
}