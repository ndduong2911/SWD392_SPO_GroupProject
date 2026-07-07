using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly NotificationStateService _notificationState; // Thêm biến này

        // Inject thêm NotificationStateService
        public NotificationService(INotificationRepository notificationRepository, NotificationStateService notificationState)
        {
            _notificationRepository = notificationRepository;
            _notificationState = notificationState;
        }

        public async Task CreateFollowNotificationAsync(Guid followerId, Guid followedUserId)
        {
            var notification = new Notification
            {
                NotifId = Guid.NewGuid(),
                UserId = followedUserId,
                Type = "FOLLOW",
                RefId = followerId,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await _notificationRepository.AddAsync(notification);

            // ĐÂY RỒI: Kích hoạt tín hiệu Real-time gửi đích danh đến người bị theo dõi!
            _notificationState.NotifyUser(followedUserId);
        }
        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
        }
     
    }
}
