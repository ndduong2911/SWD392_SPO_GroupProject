using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface IProfileService
{
    Task<UserProfile?> GetProfileAsync(Guid userId);
    Task<UserProfile> UpdateProfileAsync(Guid userId, string? displayName, string? bio, string? website, string? avatarUrl);
}
