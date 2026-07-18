using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    private static readonly HashSet<string> AllowedViolationTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "SPAM",
            "HARASSMENT",
            "VIOLENCE",
            "NUDITY",
            "COPYRIGHT",
            "OTHER"
        };

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<(bool Success, string Code, string Message)>
        SubmitReportAsync(
            Guid reporterId,
            Guid targetId,
            string targetType,
            string violationType,
            string reason)
    {
        // Precondition: User must be logged in
        if (reporterId == Guid.Empty)
        {
            return (
                false,
                "AUTH_REQUIRED",
                "Bạn cần đăng nhập để gửi báo cáo."
            );
        }

        if (targetId == Guid.Empty)
        {
            return (
                false,
                "TARGET_UNAVAILABLE",
                "Nội dung này không còn khả dụng."
            );
        }

        targetType = targetType?
            .Trim()
            .ToUpperInvariant() ?? string.Empty;

        violationType = violationType?
            .Trim()
            .ToUpperInvariant() ?? string.Empty;

        reason = reason?.Trim() ?? string.Empty;

        if (targetType is not ("PHOTO" or "PROFILE"))
        {
            return (
                false,
                "INVALID_TARGET_TYPE",
                "Loại nội dung báo cáo không hợp lệ."
            );
        }

        // A1: No violation type selected
        if (string.IsNullOrWhiteSpace(violationType))
        {
            return (
                false,
                "NO_VIOLATION_TYPE",
                "Vui lòng chọn loại vi phạm."
            );
        }

        // Ngăn người dùng sửa giá trị select từ phía client
        if (!AllowedViolationTypes.Contains(violationType))
        {
            return (
                false,
                "INVALID_VIOLATION_TYPE",
                "Loại vi phạm không hợp lệ."
            );
        }

        // A4: Invalid reason
        if (string.IsNullOrWhiteSpace(reason))
        {
            return (
                false,
                "INVALID_REASON",
                "Vui lòng nhập lý do báo cáo."
            );
        }

        if (reason.Length > 500)
        {
            return (
                false,
                "INVALID_REASON",
                "Lý do báo cáo không được vượt quá 500 ký tự."
            );
        }

        if (ContainsInvalidCharacters(reason))
        {
            return (
                false,
                "INVALID_REASON",
                "Lý do báo cáo chứa ký tự không hợp lệ."
            );
        }

        // A3: Target deleted or unavailable
        var targetOwnerId =
            await _reportRepository.GetTargetOwnerIdAsync(
                targetId,
                targetType);

        if (targetOwnerId == null)
        {
            return (
                false,
                "TARGET_UNAVAILABLE",
                "Nội dung này không còn khả dụng."
            );
        }

        // BR-01: User cannot report own content
        if (targetOwnerId.Value == reporterId)
        {
            return (
                false,
                "OWN_TARGET",
                "Bạn không thể báo cáo nội dung của chính mình."
            );
        }

        // A2 và BR-02: Only one report per target
        var hasReported =
            await _reportRepository.HasReportedAsync(
                reporterId,
                targetId,
                targetType);

        if (hasReported)
        {
            return (
                false,
                "DUPLICATE_REPORT",
                "Bạn đã báo cáo nội dung này trước đó."
            );
        }

        var report = new Report
        {
            ReportId = Guid.NewGuid(),
            ReporterId = reporterId,
            TargetId = targetId,
            TargetType = targetType,

            // Không thêm cột mới, ghép violation type và reason
            Reason = $"{violationType}|{reason}",

            // Success postcondition
            Status = "PENDING",
            CreatedAt = DateTime.Now
        };

        await _reportRepository.AddAsync(report);

        // BR-03:
        // Không có câu lệnh nào sửa hoặc xóa Photo/Profile.

        return (
            true,
            "SUCCESS",
            "Báo cáo đã được gửi thành công."
        );
    }

    public Task<IReadOnlyList<Report>> GetPendingReportsAsync()
    {
        return _reportRepository.GetPendingReportsAsync();
    }

    private static bool ContainsInvalidCharacters(string value)
    {
        return value.Any(character =>
            char.IsControl(character) &&
            character != '\r' &&
            character != '\n' &&
            character != '\t');
    }
}