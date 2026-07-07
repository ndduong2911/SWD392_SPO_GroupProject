using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public interface IFollowRepository
{
    Task<Follow> AddAsync(Follow follow);
    Task<bool> DeleteByUsersAsync(Guid followerId, Guid followingId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
    Task<int> CountFollowersAsync(Guid userId);
    Task<int> CountFollowingAsync(Guid userId);
}
