using backend.Models;
using System.Threading;
using System.Threading.Tasks;

namespace backend.Services.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(OcrJob job);
        ValueTask<OcrJob> DequeueAsync(CancellationToken cancellationToken);
    }
}