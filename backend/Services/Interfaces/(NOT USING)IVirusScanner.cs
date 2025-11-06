namespace backend.Services.Interfaces
{
    public interface IVirusScanner
    {
        /// <summary>
        /// Scan file at path. Return true if clean, false if infected or error.
        /// </summary>
        Task<bool> ScanAsync(string FullPath);
    }
}