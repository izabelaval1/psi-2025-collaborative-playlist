using MyApi.Models;

namespace MyApi.Utils
{
    public static class PlaylistSongExtensions
    {
        // Safely get the next position (handles empty list and null positions)
        public static int NextPosition(this IEnumerable<PlaylistSong> items) =>
            (items.Max(ps => ps.Position) ?? 0) + 1;

        // Order playlist songs by position (nulls go last)
        public static IEnumerable<PlaylistSong> OrderedByPosition(this IEnumerable<PlaylistSong> items) =>
            items.OrderBy(ps => ps.Position ?? int.MaxValue);
    }
}
