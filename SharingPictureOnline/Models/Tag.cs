using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class Tag
{
    public Guid TagId { get; set; }

    public string Name { get; set; } = null!;

    public int UsageCount { get; set; }

    public virtual ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();
}
