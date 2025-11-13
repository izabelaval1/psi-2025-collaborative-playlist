namespace MyApi.Models
{
    public struct Duration
    {
        public int Seconds { get; }

        public Duration(int seconds)
        {
            if (seconds < 0)
                throw new ArgumentException("Duration cannot be negative", nameof(seconds));
            Seconds = seconds;
        }

        // Create from milliseconds (what Spotify returns)
        public static Duration FromMilliseconds(int milliseconds)
        {
            return new Duration(milliseconds / 1000);
        }

        // Format as MM:SS
        public override string ToString()
        {
            var minutes = Seconds / 60;
            var secs = Seconds % 60;
            return $"{minutes}:{secs:D2}";
        }

        // Useful properties
        public int Minutes => Seconds / 60;
        public int TotalMilliseconds => Seconds * 1000;

        // Comparison operators
        public static bool operator >(Duration a, Duration b) => a.Seconds > b.Seconds;
        public static bool operator <(Duration a, Duration b) => a.Seconds < b.Seconds;
    }
}