using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly INotificationService _notificationService;
    private readonly NotificationStateService _notificationState; 

    public FollowService(IFollowRepository followRepository, INotificationService notificationService, NotificationStateService notificationState)
    {
        _followRepository = followRepository;
        _notificationService = notificationService;
        _notificationState = notificationState;
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
    {
        return await _followRepository.IsFollowingAsync(followerId, followingId);
    }

    public async Task<bool> ToggleFollowAsync(Guid followerId, Guid followingId)
    {
        if (followerId == followingId) return false;

        bool isAlreadyFollowing = await _followRepository.IsFollowingAsync(followerId, followingId);

        if (isAlreadyFollowing)
        {
            await _followRepository.DeleteByUsersAsync(followerId, followingId);

            _notificationState.NotifyFollowStatsChanged(followingId);

            return false;
        }
        else
        {
            var newFollow = new Follow
            {
                FollowId = Guid.NewGuid(),
                FollowerId = followerId,
                FollowingId = followingId,
                FollowedAt = DateTime.Now
            };
            await _followRepository.AddAsync(newFollow);
            await _notificationService.CreateFollowNotificationAsync(followerId, followingId);

            _notificationState.NotifyFollowStatsChanged(followingId);

            return true;
        }
    }

    public async Task<int> GetFollowersCountAsync(Guid userId)
    {
        return await _followRepository.CountFollowersAsync(userId);
    }

    public async Task<int> GetFollowingCountAsync(Guid userId)
    {
        return await _followRepository.CountFollowingAsync(userId);
    }
}