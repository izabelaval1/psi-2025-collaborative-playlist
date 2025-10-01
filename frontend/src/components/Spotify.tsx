export interface Artist {
  name: string;
  id: string;
  external_urls: { spotify: string };
}

export interface Album {
  name: string;
  images: { url: string }[];
}

export interface Track {
  id: string;
  name: string;
  artists: Artist[];
  album: Album;
  external_urls: { spotify: string };
}

export interface Song {
  Title: string;
  Artist: string;
  Album: string;
  Url: string;
}

export interface SpotifyResponse {
  tracks: {
    items: Track[];
  };
}
