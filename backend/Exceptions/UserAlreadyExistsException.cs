namespace MyApi.Exceptions
{
public class UserAlreadyExistsException : AppException
    {
        public string Username { get; }

        public UserAlreadyExistsException(string username) 
            : base($"Username '{username}' already exists", "USER_EXISTS")
        {
            Username = username;
            Log();
        }

        protected override string GetAdditionalInfo() => $" | Username: {Username}";
    }
}