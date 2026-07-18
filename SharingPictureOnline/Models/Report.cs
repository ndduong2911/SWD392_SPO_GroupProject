using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharingPictureOnline.Models;

public enum ReportTargetType
{
    Photo,
    Profile
}
public enum ReportStatus
{
    Pending,
    Accepted,
    Rejected
}

public partial class Report
{
    [Key]
    public Guid ReportId { get; set; } = Guid.NewGuid();

    public Guid ReporterId { get; set; }

    public string TargetType { get; set; } = null!;

    public Guid TargetId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User Reporter { get; set; } = null!;
}
