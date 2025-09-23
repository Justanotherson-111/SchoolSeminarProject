namespace backend.Services.Interfaces
{
    public interface IImageService
    {
        Task<(string RelativePath, string FileName)> SaveImageAsync(IFormFile formFile);
        Task DeleteImageAsync(string RelativePath);
    }
}