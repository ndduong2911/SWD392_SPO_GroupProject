using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly SpoContext _context;

    public ReportRepository(SpoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Report>> GetAllAsync()
    {
        return await _context.Reports.Include(r => r.Reporter).ToListAsync();
    }

    public async Task<Report> AddAsync(Report report)
    {
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return false;
        _context.Reports.Remove(report);
        return await _context.SaveChangesAsync() > 0;
    }
}
