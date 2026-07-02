using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public interface IReportRepository
{
    Task<IEnumerable<Report>> GetAllAsync();
    Task<Report> AddAsync(Report report);
    Task<bool> DeleteAsync(Guid id);
}
