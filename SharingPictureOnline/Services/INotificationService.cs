namespace SharingPictureOnline.Services;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationItem>> GetForUserAsync(Guid userId, int take = 30);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid notifId, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
}

public class NotificationItem
{
    public Guid NotifId { get; set; }
    public string Type { get; set; } = null!;
    public Guid? RefId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Icon { get; set; } = "fa-solid fa-bell";
    public string? StatusLabel { get; set; }
    public string? ActionLabel { get; set; }
}
