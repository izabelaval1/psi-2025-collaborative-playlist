namespace MyApi.Exceptions
{
    /// Custom exception type used for Spotify API errors.
    public class SpotifyServiceException : Exception
    {
        public SpotifyServiceException(string message)
            : base(message)
        {
        }
    }
}