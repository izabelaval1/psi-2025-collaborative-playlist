using MyApi.Models;

namespace MyApi.Services
{
    public interface ITokenService
    {
        string Generate(User user);
    }
}
