using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly SpoContext _context;

    public FollowRepository(SpoContext context)
    {
        _context = context;
    }

    public async Task<Follow> AddAsync(Follow follow)
    {
        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();
        return follow;
    }

    public async Task<bool> DeleteByUsersAsync(Guid followerId, Guid followingId)
    {
        var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
        if (follow == null) return false;

        _context.Follows.Remove(follow);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
    {
        return await _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    public async Task<int> CountFollowersAsync(Guid userId)
    {
        return await _context.Follows.CountAsync(f => f.FollowingId == userId);
    }

    public async Task<int> CountFollowingAsync(Guid userId)
    {
        return await _context.Follows.CountAsync(f => f.FollowerId == userId);
    }
}