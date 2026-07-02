using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string usernameOrEmail, string password);
    Task<User> RegisterAsync(string username, string email, string password);
    Task<User?> GetCurrentUserAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
}
