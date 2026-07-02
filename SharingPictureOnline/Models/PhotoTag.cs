using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class PhotoTag
{
    public Guid PhotoTagId { get; set; }

    public Guid PhotoId { get; set; }

    public Guid TagId { get; set; }

    public virtual Photo Photo { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
