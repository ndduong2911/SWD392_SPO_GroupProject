using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public interface IPhotoRepository
{
    Task<IEnumerable<Photo>> GetAllAsync();
    Task<Photo?> GetByIdAsync(Guid id);
    Task<Photo> AddAsync(Photo photo);
    Task<bool> UpdateAsync(Photo photo);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId, bool includeHidden = false);
}
