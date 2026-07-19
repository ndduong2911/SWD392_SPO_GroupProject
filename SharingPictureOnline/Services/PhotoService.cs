using Microsoft.EntityFrameworkCore;
using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services;

public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository _photoRepository;

    public PhotoService(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    public async Task<IEnumerable<Photo>> GetAllPhotosAsync()
    {
        return await _photoRepository.GetAllAsync();
    }

    public async Task<Photo?> GetPhotoByIdAsync(Guid id)
    {
        return await _photoRepository.GetByIdAsync(id);
    }

    public async Task<Photo> CreatePhotoAsync(Photo photo)
    {
        return await _photoRepository.AddAsync(photo);
    }

    public async Task<bool> UpdatePhotoAsync(Photo photo)
    {
        return await _photoRepository.UpdateAsync(photo);
    }

    public async Task<bool> DeletePhotoAsync(Guid id)
    {
        return await _photoRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Photo>> GetPhotosByUserIdAsync(Guid userId, bool includeHidden = false)
    {
        return await _photoRepository.GetByUserIdAsync(userId, includeHidden);
    }
}
