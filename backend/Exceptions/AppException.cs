namespace MyApi.Exceptions
{
    // Base exception
    public abstract class AppException : Exception
    {
        public DateTime OccurredAt { get; }
        public string ErrorCode { get; }

        protected AppException(string message, string errorCode) 
            : base(message)
        {
            OccurredAt = DateTime.UtcNow;
            ErrorCode = errorCode;
        }

        public void Log()
        {
            var logEntry = $"[{ErrorCode}] {OccurredAt:yyyy-MM-dd HH:mm:ss} - {Message}";
            logEntry += GetAdditionalInfo();
            File.AppendAllText("logs.txt", logEntry + "\n");
        }

        protected virtual string GetAdditionalInfo() => string.Empty;
    }

}