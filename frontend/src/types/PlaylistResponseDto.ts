import type { UserDto } from "./UserDto";
import type { SongDto } from "./SongDto";

export interface PlaylistResponseDto {
  id: number;
  name: string;
  description?: string;
  hostId?: number;
  host?: UserDto;
  songs: SongDto[];
  collaborators: UserDto[];
}