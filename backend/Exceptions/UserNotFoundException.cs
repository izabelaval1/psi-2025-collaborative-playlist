namespace MyApi.Exceptions
{
// User-specific exceptions
    public class UserNotFoundException : AppException
    {
        public int UserId { get; }

        public UserNotFoundException(int userId) 
            : base($"User with ID {userId} not found", "USER_NOT_FOUND")
        {
            UserId = userId;
            Log();
        }

        protected override string GetAdditionalInfo()
        {
            return $" | UserId: {UserId}";
        }
    }
}