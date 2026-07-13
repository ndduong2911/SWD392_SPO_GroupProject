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
        return await _context.Reports
            .Include(r => r.Reporter).ThenInclude(u => u.UserProfile)
            .Include(r => r.AuditLogs).ThenInclude(a => a.Actor)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetByStatusAsync(string status)
    {
        return await _context.Reports
            .Include(r => r.Reporter).ThenInclude(u => u.UserProfile)
            .Include(r => r.AuditLogs).ThenInclude(a => a.Actor)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Report?> GetByIdAsync(Guid id)
    {
        return await _context.Reports
            .Include(r => r.Reporter).ThenInclude(u => u.UserProfile)
            .Include(r => r.AuditLogs).ThenInclude(a => a.Actor)
            .FirstOrDefaultAsync(r => r.ReportId == id);
    }

    public async Task<Report> AddAsync(Report report)
    {
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();
        return report;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return false;
        report.Status = status;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report == null) return false;
        _context.Reports.Remove(report);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<int> CountByStatusAsync(string status)
    {
        return await _context.Reports.CountAsync(r => r.Status == status);
    }
}
