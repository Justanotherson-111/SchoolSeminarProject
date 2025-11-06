using backend.Services.Interfaces;
using Tesseract;
using Microsoft.Extensions.Options;
using backend.DTOs;

namespace backend.Services.ServiceDef
{
    public class TesseractOcrService : ITesseractOcrService
    {
        private readonly string _tessdataPath;

        public TesseractOcrService(IOptions<TesseractSettings> options)
        {
            _tessdataPath = options.Value.TessdataPath;
        }

        public async Task<string> ExtractTextAsync(string imagePath, string language = "eng")
        {
            return await Task.Run(() =>
            {
                using var engine = new TesseractEngine(_tessdataPath, language, EngineMode.Default);
                using var img = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(img);
                return page.GetText();
            });
        }
    }
}
