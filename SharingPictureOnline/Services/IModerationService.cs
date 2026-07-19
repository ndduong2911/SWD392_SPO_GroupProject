using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface IModerationService
{
    Task<IEnumerable<Report>> GetAllReportsAsync();
    Task<IEnumerable<Report>> GetReportsByStatusAsync(string status);
    Task<Report?> GetReportByIdAsync(Guid reportId);
    Task<Report> CreateReportAsync(Report report);

    /// <summary>
    /// Resolves a report with a moderation action.
    /// Action values: "DISMISS" | "WARN_USER" | "HIDE_CONTENT" | "REMOVE_CONTENT" | "SUSPEND_ACCOUNT" | "RESTORE_CONTENT"
    /// </summary>
    Task<bool> ResolveReportAsync(Guid reportId, string action, Guid actorId);

    Task<int> GetPendingCountAsync();

    // Content / account enforcement helpers
    Task HideContentAsync(string targetType, Guid targetId);
    Task RemoveContentAsync(string targetType, Guid targetId);
    Task RestoreContentAsync(string targetType, Guid targetId);
    Task SuspendAccountAsync(Guid userId);
    Task WarnUserAsync(Guid userId, Guid reportId);

    // Notifications
    Task NotifyReporterAsync(Guid reporterId, Guid reportId, string outcome);
    Task NotifyOffenderAsync(Guid offenderId, Guid reportId, string action);
}
