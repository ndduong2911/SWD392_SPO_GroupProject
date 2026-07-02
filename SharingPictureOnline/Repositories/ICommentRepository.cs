using SharingPictureOnline.Models;

namespace SharingPictureOnline.Repositories;

public interface ICommentRepository
{
    Task<Comment> AddAsync(Comment comment);
    Task<IEnumerable<Comment>> GetByPhotoIdAsync(Guid photoId);
}
