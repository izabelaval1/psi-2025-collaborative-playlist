import type { ArtistDto } from "./ArtistDto";
import type { UserDto } from "./UserDto";

export interface SongDto {
  id: number;
  title: string;
  album?: string;
  duration?: number;
  durationFormatted?: string;
  position?: number;
  artists: ArtistDto[];

  addedBy?: UserDto;
  addedAt?: string;  
}