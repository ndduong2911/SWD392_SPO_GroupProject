namespace SharingPictureOnline.Services
{
    public interface INotificationService
    {
        Task CreateFollowNotificationAsync(Guid followerId, Guid followedUserId);
        Task MarkAllAsReadAsync(Guid userId);
        Task CreateLikeNotificationAsync(Guid targetUserId, Guid photoId);
        Task CreateCommentNotificationAsync(Guid targetUserId, Guid photoId);
    }
}
