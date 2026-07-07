namespace SharingPictureOnline.Services
{
    public class NotificationStateService
    {
        public event Action<Guid>? OnNotificationReceived;
        public void NotifyUser(Guid targetUserId)
        {
            OnNotificationReceived?.Invoke(targetUserId);
        }

        // THÊM KÊNH 2: Dành cho việc cập nhật số Follow ở trang Profile
        public event Action<Guid>? OnFollowStatsChanged;
        public void NotifyFollowStatsChanged(Guid targetUserId)
        {
            OnFollowStatsChanged?.Invoke(targetUserId);
        }
    }
}
