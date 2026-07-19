using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly SpoContext _context;

    public PhotoRepository(SpoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Photo>> GetAllAsync()
    {
        return await _context.Photos.Include(p => p.User).Where(p => p.Visibility != "HIDDEN").ToListAsync();
    }

    public async Task<Photo?> GetByIdAsync(Guid id)
    {
        return await _context.Photos.Include(p => p.User).FirstOrDefaultAsync(p => p.PhotoId == id);
    }

    public async Task<Photo> AddAsync(Photo photo)
    {
        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();
        return photo;
    }

    public async Task<bool> UpdateAsync(Photo photo)
    {
        _context.Photos.Update(photo);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var photo = await _context.Photos.FindAsync(id);
        if (photo == null) return false;
        _context.Photos.Remove(photo);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId, bool includeHidden = false)
    {
        var query = _context.Photos.Where(p => p.UserId == userId);
        if (!includeHidden)
        {
            query = query.Where(p => p.Visibility != "HIDDEN");
        }
        return await query.ToListAsync();
    }
}
