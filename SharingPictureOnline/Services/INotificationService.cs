namespace SharingPictureOnline.Services
{
    public interface INotificationService
    {
        Task CreateFollowNotificationAsync(Guid followerId, Guid followedUserId);
        Task MarkAllAsReadAsync(Guid userId);
       
    }
}
