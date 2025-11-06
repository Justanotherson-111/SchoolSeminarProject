using backend.Models;
using backend.Services.Interfaces;
using System.Collections.Concurrent;

namespace backend.Services.ServiceDef
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<OcrJob> _jobs = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void EnqueueOcrJob(OcrJob job)
        {
            _jobs.Enqueue(job);
            _signal.Release();
        }

        public async Task<OcrJob?> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _jobs.TryDequeue(out var job);
            return job;
        }
    }
}
