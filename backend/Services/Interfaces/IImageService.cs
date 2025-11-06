using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IImageService
    {
        Task<Image> UploadImageAsync(Stream fileStream, string originalFileName, User user);
        Task<IEnumerable<Image>> GetImagesAsync(User user);
        Task<bool> DeleteImageAsync(Guid imageId, User user);
    }
}
