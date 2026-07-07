namespace SharingPictureOnline.Services
{
    public interface IFollowService
    {
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);

        Task<bool> ToggleFollowAsync(Guid followerId, Guid followingId);

        Task<int> GetFollowersCountAsync(Guid userId);

        Task<int> GetFollowingCountAsync(Guid userId);
    }
}
