using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public class NotificationService : INotificationService
{
    private readonly SpoContext _context;

    private static readonly Dictionary<string, string> ActionLabels = new()
    {
        ["DISMISS"]         = "Bác bỏ (không vi phạm)",
        ["WARN_USER"]       = "Cảnh cáo người dùng",
        ["HIDE_CONTENT"]    = "Ẩn nội dung",
        ["REMOVE_CONTENT"]  = "Xóa nội dung",
        ["SUSPEND_ACCOUNT"] = "Khóa tài khoản",
    };

    private static readonly Dictionary<string, string> StatusLabels = new()
    {
        ["RESOLVED"] = "Đã xử lý",
        ["REJECTED"] = "Bác bỏ",
        ["PENDING"]  = "Chờ xử lý",
    };

    public NotificationService(SpoContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<NotificationItem>> GetForUserAsync(Guid userId, int take = 30)
    {
        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(take)
            .ToListAsync();

        var reportIds = notifications
            .Where(n => n.RefId.HasValue && (n.Type == "REPORT" || n.Type == "SYSTEM"))
            .Select(n => n.RefId!.Value)
            .Distinct()
            .ToList();

        var reports = reportIds.Count == 0
            ? new Dictionary<Guid, Report>()
            : await _context.Reports
                .AsNoTracking()
                .Include(r => r.AuditLogs)
                .Where(r => reportIds.Contains(r.ReportId))
                .ToDictionaryAsync(r => r.ReportId);

        return notifications
            .Select(n => MapToItem(n, reports))
            .ToList();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkAsReadAsync(Guid notifId, Guid userId)
    {
        var notif = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotifId == notifId && n.UserId == userId);
        if (notif == null || notif.IsRead) return;

        notif.IsRead = true;
        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();
        if (unread.Count == 0) return;

        foreach (var n in unread)
            n.IsRead = true;

        await _context.SaveChangesAsync();
    }

    private static NotificationItem MapToItem(Notification n, IReadOnlyDictionary<Guid, Report> reports)
    {
        var item = new NotificationItem
        {
            NotifId = n.NotifId,
            Type = n.Type,
            RefId = n.RefId,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            Title = "Thông báo",
            Message = "Bạn có thông báo mới.",
            Icon = "fa-solid fa-bell"
        };

        if (n.RefId.HasValue && reports.TryGetValue(n.RefId.Value, out var report))
        {
            var audit = report.AuditLogs.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
            var action = audit?.Action;
            var actionLabel = action != null && ActionLabels.TryGetValue(action, out var al) ? al : action;
            var statusLabel = StatusLabels.TryGetValue(report.Status, out var sl) ? sl : report.Status;

            item.StatusLabel = statusLabel;
            item.ActionLabel = actionLabel;

            // Reporter outcome notification (Type REPORT, or SYSTEM where recipient is the reporter)
            if (n.Type == "REPORT" || (n.Type == "SYSTEM" && report.ReporterId == n.UserId))
            {
                item.Icon = report.Status == "REJECTED"
                    ? "fa-solid fa-ban"
                    : "fa-solid fa-flag-checkered";
                item.Title = "Kết quả báo cáo";

                if (report.Status == "REJECTED" || action == "DISMISS")
                {
                    item.Message = $"Báo cáo của bạn đã bị bác bỏ. Kết quả: {actionLabel ?? statusLabel}.";
                }
                else
                {
                    item.Message = $"Báo cáo của bạn đã được xử lý ({statusLabel}). Hành động: {actionLabel ?? "Đã xử lý"}.";
                }

                return item;
            }

            // Offender / system warning about moderation on a report
            if (n.Type == "SYSTEM")
            {
                item.Icon = "fa-solid fa-triangle-exclamation";
                item.Title = "Cảnh báo hệ thống";
                item.Message = action == "WARN_USER"
                    ? "Bạn đã nhận cảnh cáo do nội dung bị báo cáo vi phạm."
                    : $"Nội dung của bạn đã bị xử lý bởi quản trị viên. Hành động: {actionLabel ?? statusLabel}.";
                return item;
            }
        }

        item.Title = n.Type switch
        {
            "LIKE" => "Lượt thích mới",
            "COMMENT" => "Bình luận mới",
            "FOLLOW" => "Người theo dõi mới",
            "REPORT" => "Kết quả báo cáo",
            _ => "Thông báo hệ thống"
        };
        item.Icon = n.Type switch
        {
            "LIKE" => "fa-solid fa-heart",
            "COMMENT" => "fa-solid fa-comment",
            "FOLLOW" => "fa-solid fa-user-plus",
            "REPORT" => "fa-solid fa-flag",
            _ => "fa-solid fa-bell"
        };
        item.Message = n.Type switch
        {
            "LIKE" => "Ai đó đã thích ảnh của bạn.",
            "COMMENT" => "Ai đó đã bình luận ảnh của bạn.",
            "FOLLOW" => "Bạn có người theo dõi mới.",
            "REPORT" => "Báo cáo của bạn đã được xử lý.",
            _ => "Bạn có thông báo hệ thống mới."
        };

        return item;
    }
}
