using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public class ProfileService : IProfileService
{
    private readonly SpoContext _context;

    public ProfileService(SpoContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetProfileAsync(Guid userId)
    {
        return await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<UserProfile> UpdateProfileAsync(Guid userId, string? displayName, string? bio, string? website, string? avatarUrl)
    {
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            // Create new profile if not exists
            profile = new UserProfile
            {
                ProfileId = Guid.NewGuid(),
                UserId = userId,
                DisplayName = displayName,
                Bio = bio,
                Website = website,
                AvatarUrl = avatarUrl,
                UpdatedAt = DateTime.Now
            };
            _context.UserProfiles.Add(profile);
        }
        else
        {
            // Update existing profile
            profile.DisplayName = displayName;
            profile.Bio = bio;
            profile.Website = website;
            if (avatarUrl != null)
            {
                profile.AvatarUrl = avatarUrl;
            }
            profile.UpdatedAt = DateTime.Now;
            _context.UserProfiles.Update(profile);
        }

        await _context.SaveChangesAsync();
        return profile;
    }
}
