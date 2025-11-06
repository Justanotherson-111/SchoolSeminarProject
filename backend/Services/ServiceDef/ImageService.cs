using backend.DataBase;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backend.Services.ServiceDef
{
    public class ImageService : IImageService
    {
        private readonly AppDbContext _db;
        private readonly IBackgroundTaskQueue _queue;
        private readonly IWebHostEnvironment _env;
        private readonly UploadSettings _uploadSettings;

        public ImageService(
            AppDbContext db,
            IBackgroundTaskQueue queue,
            IWebHostEnvironment env,
            IOptions<UploadSettings> uploadSettings)
        {
            _db = db;
            _queue = queue;
            _env = env;
            _uploadSettings = uploadSettings.Value;

            // Ensure base folders exist
            Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, _uploadSettings.ImageFolder));
            Directory.CreateDirectory(Path.Combine(_env.ContentRootPath, _uploadSettings.TextFolder));
        }

        public async Task<Image> UploadImageAsync(Stream fileStream, string originalFileName, User user)
        {
            // Create user-specific upload folder
            var userDir = Path.Combine(_env.ContentRootPath, _uploadSettings.ImageFolder, user.UserName);
            Directory.CreateDirectory(userDir);

            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            var fullPath = Path.Combine(userDir, storedFileName);

            await using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fs);
            }

            var image = new Image
            {
                UserId = user.Id,
                OriginalFileName = originalFileName,
                FileName = storedFileName,
                RelativePath = Path.Combine(user.UserName, storedFileName), // store relative to Uploads/<username>
                Size = fileStream.Length
            };

            _db.Images.Add(image);
            await _db.SaveChangesAsync();

            // Enqueue OCR job
            var job = new OcrJob { ImageId = image.Id };
            _db.OcrJobs.Add(job);
            await _db.SaveChangesAsync();
            _queue.EnqueueOcrJob(job);

            return image;
        }

        public async Task<IEnumerable<Image>> GetImagesAsync(User user)
        {
            return await _db.Images
                .Include(i => i.TextFiles)
                .Where(i => i.UserId == user.Id)
                .ToListAsync();
        }

        public async Task<bool> DeleteImageAsync(Guid imageId, User user)
        {
            var image = await _db.Images
                .Include(i => i.TextFiles)
                .FirstOrDefaultAsync(i => i.Id == imageId && i.UserId == user.Id);

            if (image == null) return false;

            // Delete image file
            var fullImagePath = Path.Combine(_env.ContentRootPath, _uploadSettings.ImageFolder, image.RelativePath);
            if (File.Exists(fullImagePath)) File.Delete(fullImagePath);

            // Delete related text files
            foreach (var txt in image.TextFiles ?? new List<TextFile>())
            {
                var txtPath = Path.Combine(_env.ContentRootPath, _uploadSettings.TextFolder, txt.TxtFilePath);
                if (File.Exists(txtPath)) File.Delete(txtPath);
            }

            _db.TextFiles.RemoveRange(image.TextFiles ?? new List<TextFile>());
            _db.Images.Remove(image);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
