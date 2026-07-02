using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface IModerationService
{
    Task<IEnumerable<Report>> GetAllReportsAsync();
    Task<Report> CreateReportAsync(Report report);
    Task<bool> ResolveReportAsync(Guid reportId);
}
