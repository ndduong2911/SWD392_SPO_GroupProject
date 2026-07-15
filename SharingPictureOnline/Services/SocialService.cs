using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services;

public class SocialService : ISocialService
{
    private readonly ILikeRepository _likeRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IFollowRepository _followRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly INotificationService _notificationService;
    private readonly NotificationStateService _notificationState;

    public SocialService(ILikeRepository likeRepository, 
                         ICommentRepository commentRepository, 
                         IFollowRepository followRepository,
                         IPhotoRepository photoRepository,
                         INotificationService notificationService,
                         NotificationStateService notificationState)
    {
        _likeRepository = likeRepository;
        _commentRepository = commentRepository;
        _followRepository = followRepository;
        _photoRepository = photoRepository;
        _notificationService = notificationService;
        _notificationState = notificationState;
    }

    public async Task<bool> LikePhotoAsync(Guid userId, Guid photoId)
    {
        var like = new Like { LikeId = Guid.NewGuid(), UserId = userId, PhotoId = photoId, CreatedAt = DateTime.Now };
        var added = await _likeRepository.AddAsync(like);
        if (added != null)
        {
            var photo = await _photoRepository.GetByIdAsync(photoId);
            if (photo != null)
            {
                photo.LikeCount++;
                await _photoRepository.UpdateAsync(photo);
                
                // Tránh tự gửi thông báo cho chính mình
                if (photo.UserId != userId)
                {
                    await _notificationService.CreateLikeNotificationAsync(photo.UserId, userId);
                }
                
                // Kích hoạt cập nhật Real-time cho mọi người đang xem ảnh này
                _notificationState.NotifyPhotoUpdated(photoId);
            }
            return true;
        }
        return false;
    }

    public async Task<bool> UnlikePhotoAsync(Guid userId, Guid photoId)
    {
        var success = await _likeRepository.DeleteByUserAndPhotoAsync(userId, photoId);
        if (success)
        {
            var photo = await _photoRepository.GetByIdAsync(photoId);
            if (photo != null)
            {
                photo.LikeCount = Math.Max(0, photo.LikeCount - 1);
                await _photoRepository.UpdateAsync(photo);
                
                // Kích hoạt cập nhật Real-time cho mọi người đang xem ảnh này
                _notificationState.NotifyPhotoUpdated(photoId);
            }
        }
        return success;
    }
    
    public async Task<bool> HasLikedAsync(Guid userId, Guid photoId)
    {
        return await _likeRepository.HasLikedAsync(userId, photoId);
    }

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        comment.CreatedAt = DateTime.Now;
        var added = await _commentRepository.AddAsync(comment);
        if (added != null)
        {
            var photo = await _photoRepository.GetByIdAsync(comment.PhotoId);
            if (photo != null)
            {
                photo.CommentCount++;
                await _photoRepository.UpdateAsync(photo);
                
                if (photo.UserId != comment.UserId)
                {
                    await _notificationService.CreateCommentNotificationAsync(photo.UserId, comment.UserId);
                }
                
                // Kích hoạt cập nhật Real-time cho mọi người đang xem ảnh này
                _notificationState.NotifyPhotoUpdated(comment.PhotoId);
            }
        }
        return added!;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByPhotoIdAsync(Guid photoId)
    {
        var comments = await _commentRepository.GetByPhotoIdAsync(photoId);
        return comments.OrderBy(c => c.CreatedAt);
    }

    public async Task<bool> FollowUserAsync(Guid followerId, Guid followingId)
    {
        var follow = new Follow { FollowId = Guid.NewGuid(), FollowerId = followerId, FollowingId = followingId, FollowedAt = DateTime.Now };
        var added = await _followRepository.AddAsync(follow);
        return added != null;
    }

    public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        return await _followRepository.DeleteByUsersAsync(followerId, followingId);
    }
}
