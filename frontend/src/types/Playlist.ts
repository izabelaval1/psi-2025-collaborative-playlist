import type { Song } from './Song';

export type Playlist = {
  id: number;
  name: string;
  description?: string; // matches nullable Description in backend
  songs: Song[];        // array of Song objects
};
