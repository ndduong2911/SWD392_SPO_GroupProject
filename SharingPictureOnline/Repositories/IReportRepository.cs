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
    Task<int> CountByStatusAsync(string status);
}
