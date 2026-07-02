using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services;

public class ModerationService : IModerationService
{
    private readonly IReportRepository _reportRepository;

    public ModerationService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<Report>> GetAllReportsAsync()
    {
        return await _reportRepository.GetAllAsync();
    }

    public async Task<Report> CreateReportAsync(Report report)
    {
        return await _reportRepository.AddAsync(report);
    }

    public async Task<bool> ResolveReportAsync(Guid reportId)
    {
        return await _reportRepository.DeleteAsync(reportId);
    }
}
