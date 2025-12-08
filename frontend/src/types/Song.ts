export type Song = {
  id: number;
  title: string;
  album: string;
  durationMs?: number;
  duration?: number; // jei backend grazina seconds (optional)
  durationFormatted?: string; // jei backend grazina formatted string (optional)
  artists: { id: number; name: string }[];  

  addedBy?: {                // user who added the song
    id: number;
    username: string;
    role?: string;
    profileImage?: string;
  };

  addedAt?: string
  
  
};