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

        var targetSubjects = await ResolveTargetSubjectsAsync(reports.Values);

        return notifications
            .Select(n => MapToItem(n, reports, targetSubjects))
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

    private async Task<Dictionary<Guid, TargetSubject>> ResolveTargetSubjectsAsync(IEnumerable<Report> reports)
    {
        var list = reports.ToList();
        if (list.Count == 0)
            return new Dictionary<Guid, TargetSubject>();

        var photoIds = list.Where(r => r.TargetType == "PHOTO").Select(r => r.TargetId).Distinct().ToList();
        var commentIds = list.Where(r => r.TargetType == "COMMENT").Select(r => r.TargetId).Distinct().ToList();
        var userIds = list.Where(r => r.TargetType == "USER").Select(r => r.TargetId).Distinct().ToList();

        var photoTitles = photoIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _context.Photos.AsNoTracking()
                .Where(p => photoIds.Contains(p.PhotoId))
                .ToDictionaryAsync(p => p.PhotoId, p => p.Title);

        var comments = commentIds.Count == 0
            ? new Dictionary<Guid, (string Content, Guid PhotoId)>()
            : await _context.Comments.AsNoTracking()
                .Where(c => commentIds.Contains(c.CommentId))
                .ToDictionaryAsync(c => c.CommentId, c => (c.Content, c.PhotoId));

        var commentPhotoIds = comments.Values.Select(c => c.PhotoId).Distinct()
            .Where(id => !photoTitles.ContainsKey(id))
            .ToList();
        if (commentPhotoIds.Count > 0)
        {
            var extraTitles = await _context.Photos.AsNoTracking()
                .Where(p => commentPhotoIds.Contains(p.PhotoId))
                .ToDictionaryAsync(p => p.PhotoId, p => p.Title);
            foreach (var kv in extraTitles)
                photoTitles[kv.Key] = kv.Value;
        }

        var usernames = userIds.Count == 0
            ? new Dictionary<Guid, string>()
            : await _context.Users.AsNoTracking()
                .Where(u => userIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId, u => u.Username);

        var result = new Dictionary<Guid, TargetSubject>();
        foreach (var report in list)
        {
            result[report.ReportId] = report.TargetType switch
            {
                "PHOTO" => new TargetSubject(
                    "Ảnh",
                    photoTitles.TryGetValue(report.TargetId, out var title) && !string.IsNullOrWhiteSpace(title)
                        ? title.Trim()
                        : "ảnh đã báo cáo",
                    null),
                "COMMENT" => BuildCommentSubject(report.TargetId, comments, photoTitles),
                "USER" => BuildUserSubject(report.TargetId, usernames),
                _ => new TargetSubject(report.TargetType, "nội dung đã báo cáo", null)
            };
        }

        return result;
    }

    private static TargetSubject BuildCommentSubject(
        Guid commentId,
        IReadOnlyDictionary<Guid, (string Content, Guid PhotoId)> comments,
        IReadOnlyDictionary<Guid, string> photoTitles)
    {
        if (!comments.TryGetValue(commentId, out var c))
            return new TargetSubject("Bình luận", "bình luận đã báo cáo", null);

        var snippet = Truncate(c.Content, 48);
        var photoTitle = photoTitles.TryGetValue(c.PhotoId, out var t) && !string.IsNullOrWhiteSpace(t)
            ? t.Trim()
            : null;
        var label = photoTitle != null
            ? $"{snippet} (trên ảnh «{Truncate(photoTitle, 32)}»)"
            : snippet;
        return new TargetSubject("Bình luận", label, null);
    }

    private static TargetSubject BuildUserSubject(Guid userId, IReadOnlyDictionary<Guid, string> usernames)
    {
        if (usernames.TryGetValue(userId, out var name) && !string.IsNullOrWhiteSpace(name))
            return new TargetSubject("Tài khoản", name, $"/profile/{Uri.EscapeDataString(name)}");
        return new TargetSubject("Tài khoản", "tài khoản đã báo cáo", null);
    }

    private static NotificationItem MapToItem(
        Notification n,
        IReadOnlyDictionary<Guid, Report> reports,
        IReadOnlyDictionary<Guid, TargetSubject> targetSubjects)
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
            var subject = targetSubjects.TryGetValue(report.ReportId, out var ts)
                ? ts
                : new TargetSubject(GetTargetTypeLabel(report.TargetType), "nội dung đã báo cáo", null);
            var reasonSnippet = Truncate(report.Reason, 56);
            var reportDate = FormatReportDate(report.CreatedAt);
            var subjectPhrase = $"«{subject.Label}»";

            item.StatusLabel = statusLabel;
            item.ActionLabel = actionLabel;
            item.NavigateUrl = subject.NavigateUrl;

            // Reporter outcome notification (Type REPORT, or SYSTEM where recipient is the reporter)
            if (n.Type == "REPORT" || (n.Type == "SYSTEM" && report.ReporterId == n.UserId))
            {
                item.Icon = report.Status == "REJECTED"
                    ? "fa-solid fa-ban"
                    : "fa-solid fa-flag-checkered";
                item.Title = $"Kết quả báo cáo · {subject.TypeLabel}";

                if (report.Status == "REJECTED" || action == "DISMISS")
                {
                    item.Message =
                        $"Báo cáo {subject.TypeLabel.ToLowerInvariant()} {subjectPhrase} " +
                        $"(gửi {reportDate}, lý do: {reasonSnippet}) đã bị bác bỏ. " +
                        $"Kết quả: {actionLabel ?? statusLabel}.";
                }
                else
                {
                    item.Message =
                        $"Báo cáo {subject.TypeLabel.ToLowerInvariant()} {subjectPhrase} " +
                        $"(gửi {reportDate}, lý do: {reasonSnippet}) đã được xử lý. " +
                        $"Hành động: {actionLabel ?? "Đã xử lý"}.";
                }

                return item;
            }

            // Offender / system warning about moderation on a report
            if (n.Type == "SYSTEM")
            {
                item.Icon = "fa-solid fa-triangle-exclamation";
                item.Title = "Cảnh báo hệ thống";
                item.Message = action == "WARN_USER"
                    ? $"Bạn đã nhận cảnh cáo do {subject.TypeLabel.ToLowerInvariant()} {subjectPhrase} bị báo cáo vi phạm."
                    : $"{subject.TypeLabel} {subjectPhrase} của bạn đã bị xử lý bởi quản trị viên. Hành động: {actionLabel ?? statusLabel}.";
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

    private static string GetTargetTypeLabel(string targetType) => targetType switch
    {
        "PHOTO" => "Ảnh",
        "COMMENT" => "Bình luận",
        "USER" => "Tài khoản",
        _ => targetType
    };

    private static string FormatReportDate(DateTime createdAt)
    {
        var local = createdAt.Kind == DateTimeKind.Utc ? createdAt.ToLocalTime() : createdAt;
        return local.ToString("dd/MM/yyyy");
    }

    private static string Truncate(string? text, int maxLen)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "(không có)";
        var trimmed = text.Trim().Replace('\r', ' ').Replace('\n', ' ');
        while (trimmed.Contains("  ", StringComparison.Ordinal))
            trimmed = trimmed.Replace("  ", " ", StringComparison.Ordinal);
        if (trimmed.Length <= maxLen)
            return trimmed;
        return trimmed[..(maxLen - 1)].TrimEnd() + "…";
    }

    private sealed record TargetSubject(string TypeLabel, string Label, string? NavigateUrl);
}
