using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class Photo
{
    public Guid PhotoId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string StorageKey { get; set; } = null!;

    public string Visibility { get; set; } = null!;

    public int LikeCount { get; set; }

    public int CommentCount { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual ICollection<AlbumPhoto> AlbumPhotos { get; set; } = new List<AlbumPhoto>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();

    public virtual User User { get; set; } = null!;
}
