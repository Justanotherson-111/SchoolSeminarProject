namespace backend.Services.Interfaces
{
    public interface IOcrBackgroundService
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
