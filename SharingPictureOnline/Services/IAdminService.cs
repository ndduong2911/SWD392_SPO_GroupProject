using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface IAdminService
{
    // User Management
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid userId);
    Task UpdateUserRoleAsync(Guid userId, string newRole);
    Task DeleteUserAsync(Guid userId);
    
    // Tag Management
    Task<List<Tag>> GetAllTagsAsync();
    Task<Tag> CreateTagAsync(string name);
    Task UpdateTagAsync(Guid tagId, string name);
    Task DeleteTagAsync(Guid tagId);
    
    // Settings
    Task<List<string>> GetAvailableRolesAsync();
    
    // Dashboard Statistics
    Task<DashboardStats> GetDashboardStatsAsync();
    Task<List<Photo>> GetRecentPhotosAsync(int count = 10);
    Task<List<User>> GetRecentUsersAsync(int count = 10);
    Task<Dictionary<string, int>> GetUserRegistrationByMonthAsync();
    Task<Dictionary<string, int>> GetPhotoUploadByMonthAsync();
}

public class DashboardStats
{
    public int TotalUsers { get; set; }
    public int TotalPhotos { get; set; }
    public int TotalTags { get; set; }
    public int TotalReports { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewPhotosThisMonth { get; set; }
    public int PendingReports { get; set; }
    public int ActiveUsers { get; set; }
}
