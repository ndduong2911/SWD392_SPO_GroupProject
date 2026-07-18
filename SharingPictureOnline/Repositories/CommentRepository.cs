using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly SpoContext _context;

    public CommentRepository(SpoContext context)
    {
        _context = context;
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<IEnumerable<Comment>> GetByPhotoIdAsync(Guid photoId)
    {
        return await _context.Comments
            .Where(c => c.PhotoId == photoId && c.ParentId == null)
            .Include(c => c.User)
            .Include(c => c.InverseParent)
                .ThenInclude(r => r.User)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }
}
