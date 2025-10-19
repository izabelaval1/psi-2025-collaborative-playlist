import type { ArtistDto } from "./ArtistDto";

export interface SongDto {
  id: number;
  title: string;
  album?: string;
  duration?: number;
  position?: number;
  artists: ArtistDto[];
}