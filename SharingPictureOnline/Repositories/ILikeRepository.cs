using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public interface ILikeRepository
{
    Task<Like> AddAsync(Like like);
    Task<bool> DeleteByUserAndPhotoAsync(Guid userId, Guid photoId);
    Task<bool> HasLikedAsync(Guid userId, Guid photoId);
}
