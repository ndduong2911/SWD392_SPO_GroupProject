using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetAllAsync();
    Task<IEnumerable<Report>> GetByStatusAsync(string status);
    Task<Report?> GetByIdAsync(Guid id);
    Task<Report> AddAsync(Report report);
    Task<bool> UpdateStatusAsync(Guid id, string status);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> TargetExistsAsync(
        ReportTargetType targetType,
        Guid targetId);

    Task<bool> IsOwnerAsync(
        Guid reporterId,
        ReportTargetType targetType,
        Guid targetId);

    Task<bool> ExistsAsync(
        Guid reporterId,
        ReportTargetType targetType,
        Guid targetId);

    Task<bool> HasReportedAsync(
        Guid reporterId,
        Guid targetId,
        string targetType);

    Task<Guid?> GetTargetOwnerIdAsync(
        Guid targetId,
        string targetType);

    Task<IReadOnlyList<Report>> GetPendingReportsAsync();
    Task<int> CountByStatusAsync(string status);
}
