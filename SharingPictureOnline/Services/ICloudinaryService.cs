namespace SharingPictureOnline.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName);
    Task<bool> DeleteImageAsync(string publicId);
}
