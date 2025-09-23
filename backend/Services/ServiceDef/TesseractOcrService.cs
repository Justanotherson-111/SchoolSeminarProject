using backend.Services.Interfaces;
using Tesseract;

namespace backend.Services.ServiceDef
{
    public class TesseractOcrService : IOcrService
    {
        private readonly string _tessDataPath;
        public TesseractOcrService(IConfiguration config)
        {
            _tessDataPath = config["Tesseract:TesseractPath"];
            if (string.IsNullOrEmpty(_tessDataPath) || !Directory.Exists(_tessDataPath))
            {
                throw new DirectoryNotFoundException($"Tessdata not found at {_tessDataPath}");
            }
        }
        public Task<string> ExtractTextAsync(string imagePath, string lang = "eng")
        {
            using var engine = new TesseractEngine(_tessDataPath, lang, EngineMode.Default);
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);
            var text = page.GetText();
            return Task.FromResult(text);
        }
    }
}