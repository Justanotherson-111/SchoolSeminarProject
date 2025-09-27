using System.Threading.Channels;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.VisualBasic;

namespace backend.Services.ServiceDef
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<OcrJob> _queue;
        public BackgroundTaskQueue(int capacity = 100)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<OcrJob>(options);
        }
        public async ValueTask<OcrJob> DequeueAsync(CancellationToken cancellationToken)
        {
            var job = await _queue.Reader.ReadAsync(cancellationToken);
            return job;
        }

        public async ValueTask QueueBackgroundWorkItemAsync(OcrJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }
            await _queue.Writer.WriteAsync(job);
        }
    }
}