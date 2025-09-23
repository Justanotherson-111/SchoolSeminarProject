using backend.Services.Interfaces;
using Microsoft.AspNetCore.Routing.Constraints;

namespace backend.Services.ServiceDef
{
    public class ImageService : IImageService
    {
        private readonly string _uploadRoot;
        private readonly IVirusScanner _scanner;
        private readonly long _maxBytes;
        private static readonly HashSet<string> AllowedMimes = new()
        {
            "image/jpeg",
            "image/png",
            "image/bmp",
            "image/tiff",
            "image/webp"
        };
        private static readonly HashSet<string> AllowedExt = new()
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif", ".webp"
        };
        public ImageService(IConfiguration config, IVirusScanner scanner)
        {
            var folder = config["UploadSettings:ImageFolder"] ?? "Uploads";
            _uploadRoot = Path.IsPathRooted(folder) ? folder : Path.Combine(Directory.GetCurrentDirectory(), folder);
            if (!Directory.Exists(_uploadRoot)) Directory.CreateDirectory(_uploadRoot);
            _scanner = scanner;
            _maxBytes = long.TryParse(config["UploadSettings:MaxFileBytes"], out var m) ? m : 50_000_000L;
        }
        public Task DeleteImageAsync(string relativePath)
        {
            var full = Path.IsPathRooted(relativePath) ? relativePath : Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            if (File.Exists(full))
            {
                File.Delete(full);
            }
            return Task.CompletedTask;
        }

        public async Task<(string RelativePath, string FileName)> SaveImageAsync(IFormFile formFile)
        {
            if (formFile.Length <= 0)
            {
                throw new ArgumentException("Empty file");
            }
            if (formFile.Length > _maxBytes)
            {
                throw new ArgumentException("File too large");
            }
            if (!AllowedMimes.Contains(formFile.ContentType))
            {
                throw new ArgumentException("Unsupported content type");
            }
            var ext = Path.GetExtension(formFile.FileName);
            if (!AllowedExt.Contains(ext))
            {
                throw new ArgumentException("Unsupported file extension");
            }
            var fileName = $"{Guid.NewGuid()}{ext}";
            var rel = Path.Combine("Uploads", fileName);
            var full = Path.Combine(_uploadRoot, rel);

            using var stream = new FileStream(full, FileMode.Create);
            await formFile.CopyToAsync(stream);

            var clean = await _scanner.ScanAsync(full);
            if (!clean)
            {
                try
                {
                    if (File.Exists(full))
                    {
                        File.Delete(full);
                    }
                }
                catch {/* ignore */}
                {
                    throw new InvalidOperationException("File failed virus scan");
                }
            }
            return (rel, fileName);
        }
    }
}