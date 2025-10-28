using backend.DataBase;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class OCRBackgroundService : BackgroundService
{
    private readonly IBackgroundTaskQueue _queue;
    private readonly ILogger<OCRBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public OCRBackgroundService(IBackgroundTaskQueue queue, ILogger<OCRBackgroundService> logger, IServiceScopeFactory scopeFactory)
    {
        _queue = queue;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OCR background service starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            OcrJob job = null;
            try
            {
                job = await _queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException) { break; }

            if (job == null) continue;

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var ocr = scope.ServiceProvider.GetRequiredService<IOcrService>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<OCRBackgroundService>>();

                logger.LogInformation("Processing OCR for ImageId {ImageId}", job.ImageRecordId);

                var text = await ocr.ExtractTextAsync(job.FullPath, job.Language);
                if (!string.IsNullOrEmpty(text))
                {
                    var txtFolder = Path.Combine(Directory.GetCurrentDirectory(), "ExtractedTexts");
                    if (!Directory.Exists(txtFolder)) Directory.CreateDirectory(txtFolder);

                    var txtFileName = $"{Guid.NewGuid()}.txt";
                    var txtPath = Path.Combine(txtFolder, txtFileName);

                    await File.WriteAllTextAsync(txtPath, text);

                    var extracted = new ExtractedText
                    {
                        ImageRecordId = job.ImageRecordId,
                        Language = job.Language,
                        TxtFilePath = txtPath
                    };
                    db.ExtractedTexts.Add(extracted);
                    await db.SaveChangesAsync();

                    logger.LogInformation("OCR completed and saved to {TxtPath}", txtPath);
                }
                else
                {
                    logger.LogWarning("OCR returned empty text for {ImageId}", job.ImageRecordId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing OCR job {@Job}", job);
            }
        }

        _logger.LogInformation("OCR background service stopping.");
    }
}
