using System.Diagnostics;
using backend.Services.Interfaces;

namespace backend.Services.ServiceDef
{
    public class ClamAvVirusScanner : IVirusScanner
    {
        private readonly ILogger<ClamAvVirusScanner> _logger;
        private readonly string _clamdPath;
        private readonly string _clamscanPath;
        public ClamAvVirusScanner(ILogger<ClamAvVirusScanner> logger, IConfiguration config)
        {
            _logger = logger;
            _clamdPath = config["VirusScanner:ClamdPath"] ?? "clamdscan";
            _clamscanPath = config["VirusScanner:ClamscanPath"] ?? "clamscan";
        }
        public async Task<bool> ScanAsync(string fullPath)
        {
            try
            {
                string exe, args;
                if (File.Exists(_clamdPath))
                {
                    exe = _clamdPath;
                    args = $"--no-summary \"{fullPath}\"";
                }
                else if (File.Exists(_clamscanPath))
                {
                    exe = _clamscanPath;
                    args = $"--no-summary \"{fullPath}\"";
                }
                else
                {
                    throw new FileNotFoundException("No virus scanner binary found at configured paths.");
                }

                var psi = new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                var stdout = await proc.StandardOutput.ReadToEndAsync();
                var stderr = await proc.StandardError.ReadToEndAsync();
                await proc.WaitForExitAsync();

                _logger.LogDebug("Virus scanner stdout: {out}", stdout);
                if (!string.IsNullOrEmpty(stderr))
                {
                    _logger.LogWarning("Virus scanner stderr: {err}", stderr);
                }
                return proc.ExitCode == 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error running virus scanner; rejecting upload for safety.");
                return false;
            }
        }
    }
}