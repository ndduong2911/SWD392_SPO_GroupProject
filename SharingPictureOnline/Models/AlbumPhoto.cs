using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class AlbumPhoto
{
    public Guid AlbumPhotoId { get; set; }

    public Guid AlbumId { get; set; }

    public Guid PhotoId { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual Photo Photo { get; set; } = null!;
}
