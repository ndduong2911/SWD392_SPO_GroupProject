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
        await _context.Reports.AddAsync(report);
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

    public async Task<bool> TargetExistsAsync(
        ReportTargetType targetType,
        Guid targetId)
    {
        return targetType switch
        {
            ReportTargetType.Photo =>
                await _context.Photos
                    .AnyAsync(p => p.PhotoId == targetId),

            // Đang giả định TargetId của Profile là UserId
            ReportTargetType.Profile =>
                await _context.Users
                    .AnyAsync(u => u.UserId == targetId),

            _ => false
        };
    }

    public async Task<bool> IsOwnerAsync(
        Guid reporterId,
        ReportTargetType targetType,
        Guid targetId)
    {
        return targetType switch
        {
            ReportTargetType.Photo =>
                await _context.Photos.AnyAsync(p =>
                    p.PhotoId == targetId &&
                    p.UserId == reporterId),

            // Nếu target Profile là UserId
            ReportTargetType.Profile =>
                targetId == reporterId,

            _ => false
        };
    }

    public Task<bool> ExistsAsync(
        Guid reporterId,
        ReportTargetType targetType,
        Guid targetId)
    {
        return _context.Reports.AnyAsync(r =>
            r.ReporterId == reporterId &&
            r.TargetType == targetType.ToString() &&
            r.TargetId == targetId);
    }

    public async Task<bool> HasReportedAsync(
        Guid reporterId,
        Guid targetId,
        string targetType)
    {
        return await _context.Reports
            .AsNoTracking()
            .AnyAsync(r =>
                r.ReporterId == reporterId &&
                r.TargetId == targetId &&
                r.TargetType == targetType);
    }

    public async Task<Guid?> GetTargetOwnerIdAsync(
        Guid targetId,
        string targetType)
    {
        if (targetType == "PHOTO")
        {
            return await _context.Photos
                .AsNoTracking()
                .Where(p => p.PhotoId == targetId)
                .Select(p => (Guid?)p.UserId)
                .FirstOrDefaultAsync();
        }

        if (targetType == "PROFILE")
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .Where(p => p.ProfileId == targetId)
                .Select(p => (Guid?)p.UserId)
                .FirstOrDefaultAsync();
        }

        return null;
    }

    public async Task<IReadOnlyList<Report>> GetPendingReportsAsync()
    {
        return await _context.Reports
            .AsNoTracking()
            .Include(r => r.Reporter)
            .Where(r => r.Status == "PENDING")
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }


}
