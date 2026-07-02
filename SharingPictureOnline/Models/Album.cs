using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class Album
{
    public Guid AlbumId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? CoverPhotoId { get; set; }

    public string Visibility { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AlbumPhoto> AlbumPhotos { get; set; } = new List<AlbumPhoto>();

    public virtual User User { get; set; } = null!;
}
