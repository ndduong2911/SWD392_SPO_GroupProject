using System;

namespace SharingPictureOnline.Models;

public partial class Report
{
    public Guid ReportId { get; set; }

    public Guid ReporterId { get; set; }

    public string TargetType { get; set; } = null!;

    public Guid TargetId { get; set; }

    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User Reporter { get; set; } = null!;

    // Navigation: audit history cho report này
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
