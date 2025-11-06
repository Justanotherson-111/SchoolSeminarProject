using backend.DataBase;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace backend.Services.ServiceDef
{
    public class OcrBackgroundService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly ITesseractOcrService _ocrService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;
        private readonly UploadSettings _uploadSettings;

        public OcrBackgroundService(
            IBackgroundTaskQueue queue,
            ITesseractOcrService ocrService,
            IServiceProvider serviceProvider,
            IWebHostEnvironment env,
            IOptions<UploadSettings> uploadSettings)
        {
            _queue = queue;
            _ocrService = ocrService;
            _serviceProvider = serviceProvider;
            _env = env;
            _uploadSettings = uploadSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _queue.DequeueAsync(stoppingToken);
                if (job == null) continue;

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    
                    var image = await db.Images
                        .Include(i => i.User)
                        .FirstOrDefaultAsync(i => i.Id == job.ImageId, stoppingToken);

                    if (image == null) continue;

                    var fullImagePath = Path.Combine(_env.ContentRootPath, _uploadSettings.ImageFolder, image.RelativePath);

                    // Extract text from image
                    var text = await _ocrService.ExtractTextAsync(fullImagePath);

                    // Save text to ExtractedText/<username> folder
                    var userTextDir = Path.Combine(_env.ContentRootPath, _uploadSettings.TextFolder, image.User!.UserName);
                    Directory.CreateDirectory(userTextDir);

                    var txtFileName = $"{Guid.NewGuid()}.txt";
                    var txtFilePath = Path.Combine(userTextDir, txtFileName);
                    await File.WriteAllTextAsync(txtFilePath, text, stoppingToken);

                    // Save record to DB
                    var txtRecord = new TextFile
                    {
                        ImageId = image.Id,
                        TxtFilePath = Path.Combine(image.User.UserName, txtFileName), // store relative
                        Language = "eng"
                    };

                    db.TextFiles.Add(txtRecord);
                    job.Processed = true;
                    db.OcrJobs.Update(job);
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Optional: log the exception
                    Console.WriteLine($"OCR job failed: {ex.Message}");
                }
            }
        }
    }
}
