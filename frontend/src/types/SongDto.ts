import type { ArtistDto } from "./ArtistDto";

export interface SongDto {
  id: number;
  title: string;
  album?: string;
  duration?: number;
  durationFormatted?: string;
  position?: number;
  artists: ArtistDto[];
  
  spotifyId: string;
  spotifyUri: string;
}