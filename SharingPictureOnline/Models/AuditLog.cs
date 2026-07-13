using System;

namespace SharingPictureOnline.Models;

public partial class AuditLog
{
    public Guid LogId { get; set; }

    public Guid ReportId { get; set; }

    public Guid ActorId { get; set; }

    public string Action { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Report Report { get; set; } = null!;

    public virtual User Actor { get; set; } = null!;
}
