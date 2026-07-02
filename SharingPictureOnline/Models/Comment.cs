using System;
using System.Collections.Generic;

namespace SharingPictureOnline.Models;

public partial class Comment
{
    public Guid CommentId { get; set; }

    public Guid PhotoId { get; set; }

    public Guid UserId { get; set; }

    public Guid? ParentId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();

    public virtual Comment? Parent { get; set; }

    public virtual Photo Photo { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
