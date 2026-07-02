using SharingPictureOnline.Models;
using SharingPictureOnline.Repositories;

namespace SharingPictureOnline.Services;

public class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _albumRepository;

    public AlbumService(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public async Task<IEnumerable<Album>> GetAllAlbumsAsync()
    {
        return await _albumRepository.GetAllAsync();
    }

    public async Task<Album?> GetAlbumByIdAsync(Guid id)
    {
        return await _albumRepository.GetByIdAsync(id);
    }

    public async Task<Album> CreateAlbumAsync(Album album)
    {
        return await _albumRepository.AddAsync(album);
    }

    public async Task<bool> UpdateAlbumAsync(Album album)
    {
        return await _albumRepository.UpdateAsync(album);
    }

    public async Task<bool> DeleteAlbumAsync(Guid id)
    {
        return await _albumRepository.DeleteAsync(id);
    }
}
