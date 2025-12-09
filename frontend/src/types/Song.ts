export type Song = {
  id: number;
  title: string;
  album?: string;
  durationMs?: number;
  duration?: number;
  durationFormatted?: string;
  artists: { id: number; name: string }[];
  spotifyId: string;
  spotifyUri: string;
};