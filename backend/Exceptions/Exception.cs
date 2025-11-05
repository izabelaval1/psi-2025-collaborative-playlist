namespace MyApi.Exceptions
{
    /// <summary>
    /// Custom exception type used for Spotify API errors.
    /// </summary>
    public class SpotifyServiceException : Exception
    {
        public SpotifyServiceException(string message)
            : base(message)
        {
        }

        // ar reikia inner exception?
        public SpotifyServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}