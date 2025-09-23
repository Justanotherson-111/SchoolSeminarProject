using backend.DataBase;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace backend.Services.ServiceDef
{
    public class OCRBackgroundService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly ILogger<OCRBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public OCRBackgroundService(IBackgroundTaskQueue backgroundTaskQueue, ILogger<OCRBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _queue = backgroundTaskQueue;
            _logger = logger;
            _scopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OCR background service starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                OcrJob ocrJob = null;
                try
                {
                    ocrJob = await _queue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                if (ocrJob == null)
                {
                    continue;
                }
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var ocr = scope.ServiceProvider.GetRequiredService<IOcrService>();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<OCRBackgroundService>>();

                    logger.LogInformation("Processing OCR for ImageId {ImageId}", ocrJob.ImageRecordId);

                    var text = await ocr.ExtractTextAsync(ocrJob.FullPath, ocrJob.Language);

                    if (!string.IsNullOrEmpty(text))
                    {
                        var extracted = new Models.ExtractedText
                        {
                            ImageRecordId = ocrJob.ImageRecordId,
                            Text = text,
                            Language = ocrJob.Language
                        };
                        db.ExtractedTexts.Add(extracted);
                        await db.SaveChangesAsync();
                        logger.LogInformation("OCR completed and saved for {ImageId}", ocrJob.ImageRecordId);
                    }
                    else
                    {
                        logger.LogWarning("OCR returned empty text for {ImageId}", ocrJob.ImageRecordId);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error passing OCR job {@job}", ocrJob);
                }
            }
            _logger.LogInformation("OCR background service stopping.");
        }
    }
}