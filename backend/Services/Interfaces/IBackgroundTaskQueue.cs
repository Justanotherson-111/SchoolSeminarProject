using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void EnqueueOcrJob(OcrJob job);
        Task<OcrJob?> DequeueAsync(CancellationToken cancellationToken);
    }
}
