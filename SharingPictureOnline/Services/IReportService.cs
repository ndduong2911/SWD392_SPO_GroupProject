using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface IReportService
{
    Task<(bool Success, string Code, string Message)> SubmitReportAsync(
        Guid reporterId,
        Guid targetId,
        string targetType,
        string violationType,
        string reason);

    Task<IReadOnlyList<Report>> GetPendingReportsAsync();
}