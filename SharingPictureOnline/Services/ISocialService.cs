using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface ISocialService
{
    Task<bool> LikePhotoAsync(Guid userId, Guid photoId);
    Task<bool> UnlikePhotoAsync(Guid userId, Guid photoId);
    Task<Comment> AddCommentAsync(Comment comment);
    Task<IEnumerable<Comment>> GetCommentsByPhotoIdAsync(Guid photoId);
    Task<bool> FollowUserAsync(Guid followerId, Guid followingId);
    Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId);
    Task<bool> HasLikedAsync(Guid userId, Guid photoId);
}
