namespace MyApi.Exceptions
{
    // Custom exception type used for Spotify API errors.
    public class SpotifyServiceException : AppException
    {
        public int? StatusCode { get; }
        public string? ResponseBody { get; }

        public SpotifyServiceException(string message) 
            : base(message, "SPOTIFY_ERROR")
        {
            Log();
        }

        public SpotifyServiceException(string message, int statusCode, string? responseBody = null) 
            : base(message, "SPOTIFY_API_ERROR")
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
            Log();
        }

        protected override string GetAdditionalInfo()
        {
            var info = StatusCode.HasValue ? $" | StatusCode: {StatusCode}" : "";
            if (!string.IsNullOrEmpty(ResponseBody))
                info += $" | Response: {ResponseBody.Substring(0, Math.Min(100, ResponseBody.Length))}...";
            return info;
        }
    }
}