using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public interface IAlbumRepository
{
    Task<IEnumerable<Album>> GetAllAsync();
    Task<Album?> GetByIdAsync(Guid id);
    Task<Album> AddAsync(Album album);
    Task<bool> UpdateAsync(Album album);
    Task<bool> DeleteAsync(Guid id);
}
