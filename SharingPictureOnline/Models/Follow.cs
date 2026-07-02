using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class Follow
{
    public Guid FollowId { get; set; }

    public Guid FollowerId { get; set; }

    public Guid FollowingId { get; set; }

    public DateTime FollowedAt { get; set; }

    public virtual User Follower { get; set; } = null!;

    public virtual User Following { get; set; } = null!;
}
