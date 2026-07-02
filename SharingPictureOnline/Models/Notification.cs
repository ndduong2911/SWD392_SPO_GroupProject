using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class Notification
{
    public Guid NotifId { get; set; }

    public Guid UserId { get; set; }

    public string Type { get; set; } = null!;

    public Guid? RefId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
