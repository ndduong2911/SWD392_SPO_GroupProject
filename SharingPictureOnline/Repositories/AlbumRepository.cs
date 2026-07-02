using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly SpoContext _context;

    public AlbumRepository(SpoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Album>> GetAllAsync()
    {
        return await _context.Albums.Include(a => a.User).ToListAsync();
    }

    public async Task<Album?> GetByIdAsync(Guid id)
    {
        return await _context.Albums.Include(a => a.User).FirstOrDefaultAsync(a => a.AlbumId == id);
    }

    public async Task<Album> AddAsync(Album album)
    {
        _context.Albums.Add(album);
        await _context.SaveChangesAsync();
        return album;
    }

    public async Task<bool> UpdateAsync(Album album)
    {
        _context.Albums.Update(album);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var album = await _context.Albums.FindAsync(id);
        if (album == null) return false;
        _context.Albums.Remove(album);
        return await _context.SaveChangesAsync() > 0;
    }
}
