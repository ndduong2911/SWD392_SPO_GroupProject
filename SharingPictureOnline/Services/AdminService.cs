using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public class AdminService : IAdminService
{
    private readonly SpoContext _context;

    public AdminService(SpoContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.UserProfile)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateUserRoleAsync(Guid userId, string newRole)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Role = newRole;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Tag>> GetAllTagsAsync()
    {
        return await _context.Tags
            .OrderByDescending(t => t.UsageCount)
            .ToListAsync();
    }

    public async Task<Tag> CreateTagAsync(string name)
    {
        var tag = new Tag
        {
            TagId = Guid.NewGuid(),
            Name = name.ToLower().Trim(),
            UsageCount = 0
        };
        
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task UpdateTagAsync(Guid tagId, string name)
    {
        var tag = await _context.Tags.FindAsync(tagId);
        if (tag != null)
        {
            tag.Name = name.ToLower().Trim();
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteTagAsync(Guid tagId)
    {
        var tag = await _context.Tags.FindAsync(tagId);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }

    public Task<List<string>> GetAvailableRolesAsync()
    {
        return Task.FromResult(new List<string> { "ADMIN", "MODERATOR", "USER" });
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var now = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

        var stats = new DashboardStats
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalPhotos = await _context.Photos.CountAsync(),
            TotalTags = await _context.Tags.CountAsync(),
            TotalReports = await _context.Reports.CountAsync(),
            NewUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= firstDayOfMonth),
            NewPhotosThisMonth = await _context.Photos.CountAsync(p => p.UploadedAt >= firstDayOfMonth),
            PendingReports = await _context.Reports.CountAsync(r => r.Status == "PENDING"),
            ActiveUsers = await _context.Users.CountAsync(u => u.CreatedAt >= now.AddDays(-30))
        };

        return stats;
    }

    public async Task<List<Photo>> GetRecentPhotosAsync(int count = 10)
    {
        return await _context.Photos
            .Include(p => p.User)
            .OrderByDescending(p => p.UploadedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<User>> GetRecentUsersAsync(int count = 10)
    {
        return await _context.Users
            .Include(u => u.UserProfile)
            .OrderByDescending(u => u.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetUserRegistrationByMonthAsync()
    {
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        
        var users = await _context.Users
            .Where(u => u.CreatedAt >= sixMonthsAgo)
            .ToListAsync();

        var grouped = users
            .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .ToDictionary(
                g => $"{g.Key.Month}/{g.Key.Year}",
                g => g.Count()
            );

        return grouped;
    }

    public async Task<Dictionary<string, int>> GetPhotoUploadByMonthAsync()
    {
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        
        var photos = await _context.Photos
            .Where(p => p.UploadedAt >= sixMonthsAgo)
            .ToListAsync();

        var grouped = photos
            .GroupBy(p => new { p.UploadedAt.Year, p.UploadedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .ToDictionary(
                g => $"{g.Key.Month}/{g.Key.Year}",
                g => g.Count()
            );

        return grouped;
    }
}
