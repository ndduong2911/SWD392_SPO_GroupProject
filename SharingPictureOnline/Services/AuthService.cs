using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public class AuthService : IAuthService
{
    private readonly SpoContext _context;
    private readonly ProtectedSessionStorage _sessionStorage;
    private User? _cachedUser;

    public AuthService(SpoContext context, ProtectedSessionStorage sessionStorage)
    {
        _context = context;
        _sessionStorage = sessionStorage;
    }

    public bool IsAuthenticated => _cachedUser != null;

    public async Task<User?> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _context.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => 
                u.Username == usernameOrEmail || u.Email == usernameOrEmail);

        if (user != null && PasswordHasher.VerifyPassword(password, user.PasswordHash))
        {
            _cachedUser = user;
            await _sessionStorage.SetAsync("userId", user.UserId.ToString());
            return user;
        }

        return null;
    }

    public async Task<User> RegisterAsync(string username, string email, string password)
    {
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = PasswordHasher.HashPassword(password),
            Role = "USER",
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _cachedUser = user;
        await _sessionStorage.SetAsync("userId", user.UserId.ToString());
        return user;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_cachedUser != null)
            return _cachedUser;

        for (var attempt = 0; attempt < 2; attempt++)
        {
            try
            {
                var result = await _sessionStorage.GetAsync<string>("userId");
                if (result.Success && !string.IsNullOrEmpty(result.Value))
                {
                    if (Guid.TryParse(result.Value, out var userId))
                    {
                        _cachedUser = await _context.Users
                            .Include(u => u.UserProfile)
                            .FirstOrDefaultAsync(u => u.UserId == userId);
                        return _cachedUser;
                    }
                }

                return null;
            }
            catch
            {
                if (attempt == 0)
                {
                    await Task.Delay(100);
                    continue;
                }
            }
        }

        return null;
    }

    public async Task LogoutAsync()
    {
        _cachedUser = null;
        await _sessionStorage.DeleteAsync("userId");
    }
}
