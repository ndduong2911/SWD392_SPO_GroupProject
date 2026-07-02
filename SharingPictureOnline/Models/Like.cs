using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class Like
{
    public Guid LikeId { get; set; }

    public Guid UserId { get; set; }

    public Guid PhotoId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Photo Photo { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
