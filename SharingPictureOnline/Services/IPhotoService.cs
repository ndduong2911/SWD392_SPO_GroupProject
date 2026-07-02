using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface IPhotoService
{
    Task<IEnumerable<Photo>> GetAllPhotosAsync();
    Task<Photo?> GetPhotoByIdAsync(Guid id);
    Task<Photo> CreatePhotoAsync(Photo photo);
    Task<bool> UpdatePhotoAsync(Photo photo);
    Task<bool> DeletePhotoAsync(Guid id);
    Task<IEnumerable<Photo>> GetPhotosByUserIdAsync(Guid userId);
}
