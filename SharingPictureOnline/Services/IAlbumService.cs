using SharingPictureOnline.Models;

namespace SharingPictureOnline.Services;

public interface IAlbumService
{
    Task<IEnumerable<Album>> GetAllAlbumsAsync();
    Task<Album?> GetAlbumByIdAsync(Guid id);
    Task<Album> CreateAlbumAsync(Album album);
    Task<bool> UpdateAlbumAsync(Album album);
    Task<bool> DeleteAlbumAsync(Guid id);
}
