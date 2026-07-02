using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly SpoContext _context;

    public LikeRepository(SpoContext context)
    {
        _context = context;
    }

    public async Task<Like> AddAsync(Like like)
    {
        _context.Likes.Add(like);
        await _context.SaveChangesAsync();
        return like;
    }

    public async Task<bool> DeleteByUserAndPhotoAsync(Guid userId, Guid photoId)
    {
        var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.PhotoId == photoId);
        if (like == null) return false;
        _context.Likes.Remove(like);
        return await _context.SaveChangesAsync() > 0;
    }
}
