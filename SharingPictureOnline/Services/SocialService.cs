using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services;

public class SocialService : ISocialService
{
    private readonly ILikeRepository _likeRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IFollowRepository _followRepository;

    public SocialService(ILikeRepository likeRepository, ICommentRepository commentRepository, IFollowRepository followRepository)
    {
        _likeRepository = likeRepository;
        _commentRepository = commentRepository;
        _followRepository = followRepository;
    }

    public async Task<bool> LikePhotoAsync(Guid userId, Guid photoId)
    {
        var like = new Like { UserId = userId, PhotoId = photoId, CreatedAt = DateTime.Now };
        await _likeRepository.AddAsync(like);
        return true;
    }

    public async Task<bool> UnlikePhotoAsync(Guid userId, Guid photoId)
    {
        return await _likeRepository.DeleteByUserAndPhotoAsync(userId, photoId);
    }

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        comment.CreatedAt = DateTime.Now;
        return await _commentRepository.AddAsync(comment);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByPhotoIdAsync(Guid photoId)
    {
        return await _commentRepository.GetByPhotoIdAsync(photoId);
    }

    public async Task<bool> FollowUserAsync(Guid followerId, Guid followingId)
    {
        var follow = new Follow { FollowerId = followerId, FollowingId = followingId, FollowedAt = DateTime.Now };
        await _followRepository.AddAsync(follow);
        return true;
    }

    public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId)
    {
        return await _followRepository.DeleteByUsersAsync(followerId, followingId);
    }
}
