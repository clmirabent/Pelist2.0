namespace Films.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);
}
