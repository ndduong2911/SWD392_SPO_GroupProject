using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class UserProfile
{
    public Guid ProfileId { get; set; }

    public Guid UserId { get; set; }

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Website { get; set; }

    public string? DisplayName { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
