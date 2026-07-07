using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> AddAsync(Notification notification);

        // Hai hàm này để sau này bạn làm giao diện cái chuông thông báo
        Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId);
        Task<int> CountUnreadAsync(Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
