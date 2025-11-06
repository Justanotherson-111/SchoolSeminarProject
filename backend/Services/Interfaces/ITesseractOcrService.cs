namespace backend.Services.Interfaces
{
    public interface ITesseractOcrService
    {
        Task<string> ExtractTextAsync(string imagePath, string language = "eng");
    }
}
