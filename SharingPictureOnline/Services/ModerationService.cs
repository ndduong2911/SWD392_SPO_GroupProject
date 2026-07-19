using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services;

public class ModerationService : IModerationService
{
    private readonly IReportRepository _reportRepository;
    private readonly SpoContext _context;

    public ModerationService(IReportRepository reportRepository, SpoContext context)
    {
        _reportRepository = reportRepository;
        _context = context;
    }

    public async Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        return await _reportRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Report>> GetReportsByStatusAsync(string status)
    {
        return await _reportRepository.GetByStatusAsync(status);
    }

    public async Task<Report?> GetReportByIdAsync(Guid reportId)
    {
        return await _reportRepository.GetByIdAsync(reportId);
    }

    public async Task<Report> CreateReportAsync(Report report)
    {
        return await _reportRepository.AddAsync(report);
    }

    public async Task<bool> ResolveReportAsync(Guid reportId, string action, Guid actorId)
    {
        var report = await _reportRepository.GetByIdAsync(reportId);
        if (report == null) return false;

        // BR-07: actor cannot be the reporter
        if (report.ReporterId == actorId) return false;

        string newStatus = action == "DISMISS" ? "REJECTED" : "RESOLVED";
        using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            if (action != "DISMISS")
            {
                switch (action)
                {
                    case "WARN_USER":
                        var offenderId = await GetOffenderIdAsync(report.TargetType, report.TargetId);
                        if (offenderId.HasValue)
                            await WarnUserAsync(offenderId.Value, reportId);
                        break;

                    case "HIDE_CONTENT":
                        await HideContentAsync(report.TargetType, report.TargetId);
                        break;

                    case "REMOVE_CONTENT":
                        await RemoveContentAsync(report.TargetType, report.TargetId);
                        break;

                    case "RESTORE_CONTENT":
                        await RestoreContentAsync(report.TargetType, report.TargetId);
                        break;

                    case "SUSPEND_ACCOUNT":
                        var suspendId = await GetOffenderIdAsync(report.TargetType, report.TargetId);
                        if (suspendId.HasValue)
                            await SuspendAccountAsync(suspendId.Value);
                        break;
                }
            }

            // Update report status
            await _reportRepository.UpdateStatusAsync(reportId, newStatus);

            // BR-06: Write audit log with actor ID and timestamp
            var auditLog = new AuditLog
            {
                LogId = Guid.NewGuid(),
                ReportId = reportId,
                ActorId = actorId,
                Action = action,
                Note = $"Report {newStatus} — action: {action}",
                CreatedAt = DateTime.Now
            };
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            await NotifyReporterAsync(report.ReporterId, reportId, action);

            // Notify offender if action was taken
            if (action != "DISMISS")
            {
                var offId = await GetOffenderIdAsync(report.TargetType, report.TargetId);
                if (offId.HasValue && offId.Value != report.ReporterId)
                    await NotifyOffenderAsync(offId.Value, reportId, action);
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return true;
    }

    public async Task<int> GetPendingCountAsync()
    {
        return await _reportRepository.CountByStatusAsync("PENDING");
    }

    // ── Content enforcement ──────────────────────────────────────────────────

    public async Task HideContentAsync(string targetType, Guid targetId)
    {
        if (targetType == "PHOTO")
        {
            var photo = await _context.Photos.FindAsync(targetId);
            if (photo != null)
            {
                photo.Visibility = "HIDDEN";
                await _context.SaveChangesAsync();
            }
        }
        else if (targetType == "COMMENT")
        {
            await RemoveContentAsync(targetType, targetId);
        }
    }

    public async Task RemoveContentAsync(string targetType, Guid targetId)
    {
        if (targetType == "PHOTO")
        {
            // Physically remove the photo DB record and any related comments.
            var photo = await _context.Photos
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PhotoId == targetId);

            if (photo != null)
            {
                // Remove comments associated with this photo to avoid FK conflicts
                var comments = await _context.Comments
                    .Where(c => c.PhotoId == targetId)
                    .ToListAsync();
                if (comments.Count > 0)
                    _context.Comments.RemoveRange(comments);

                _context.Photos.Remove(photo);

                // If you store files externally (Cloudinary/Azure/S3), delete storage object here.

                await _context.SaveChangesAsync();
            }
        }
        else if (targetType == "COMMENT")
        {
            var comment = await _context.Comments.FindAsync(targetId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task RestoreContentAsync(string targetType, Guid targetId)
    {
        if (targetType == "PHOTO")
        {
            var photo = await _context.Photos.FindAsync(targetId);
            if (photo != null)
            {
                // restore visibility to a visible state used by the app
                photo.Visibility = "PUBLIC";
                await _context.SaveChangesAsync();
            }
            // if photo == null then it was removed; restoration is not possible here
        }
        else if (targetType == "COMMENT")
        {
            // Comments are permanently removed by RemoveContentAsync — to support comment restore,
            // implement soft-delete on comments and update here.
        }
    }

    public async Task SuspendAccountAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Role = "BANNED";
            await _context.SaveChangesAsync();
        }
    }

    public async Task WarnUserAsync(Guid userId, Guid reportId)
    {
        var notification = new Notification
        {
            NotifId = Guid.NewGuid(),
            UserId = userId,
            Type = "SYSTEM",
            RefId = reportId,
            IsRead = false,
            CreatedAt = DateTime.Now
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    // ── Notifications ────────────────────────────────────────────────────────

    public async Task NotifyReporterAsync(Guid reporterId, Guid reportId, string outcome)
    {
        var notification = new Notification
        {
            NotifId = Guid.NewGuid(),
            UserId = reporterId,
            Type = "REPORT",
            RefId = reportId,
            IsRead = false,
            CreatedAt = DateTime.Now
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task NotifyOffenderAsync(Guid offenderId, Guid reportId, string action)
    {
        var notification = new Notification
        {
            NotifId = Guid.NewGuid(),
            UserId = offenderId,
            Type = "SYSTEM",
            RefId = reportId,
            IsRead = false,
            CreatedAt = DateTime.Now
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<Guid?> GetOffenderIdAsync(string targetType, Guid targetId)
    {
        if (targetType == "PHOTO")
        {
            var photo = await _context.Photos.FindAsync(targetId);
            return photo?.UserId;
        }
        if (targetType == "COMMENT")
        {
            var comment = await _context.Comments.FindAsync(targetId);
            return comment?.UserId;
        }
        if (targetType == "PROFILE")
        {
            var profile = await _context.UserProfiles.FindAsync(targetId);
            return profile?.UserId;
        }
        if (targetType == "USER")
        {
            return targetId;
        }
        return null;
    }
}
