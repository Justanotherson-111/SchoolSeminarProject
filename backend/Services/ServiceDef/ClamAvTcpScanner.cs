using System.Net;
using System.Net.Sockets;
using System.Text;
using backend.Services.Interfaces;

namespace backend.Services.ServiceDef
{
    public class ClamAvTcpScanner : IVirusScanner
    {
        private readonly string _host;
        private readonly int _port;
        private readonly ILogger<ClamAvTcpScanner> _logger;

        public ClamAvTcpScanner(IConfiguration config, ILogger<ClamAvTcpScanner> logger)
        {
            _host = config["ClamAV:Host"] ?? "clamav";
            _port = int.TryParse(config["ClamAV:Port"], out var p) ? p : 3310;
            _logger = logger;
        }

        public async Task<bool> ScanAsync(string fullPath)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(_host, _port);
                using var ns = client.GetStream();

                var writer = new StreamWriter(ns, Encoding.ASCII) { AutoFlush = true };
                await writer.WriteLineAsync("INSTREAM");

                await using (var fs = File.OpenRead(fullPath))
                {
                    var buffer = new byte[2048];
                    int read;
                    while ((read = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        var size = IPAddress.HostToNetworkOrder(read);
                        var sizeBytes = BitConverter.GetBytes(size);
                        await ns.WriteAsync(sizeBytes, 0, 4);
                        await ns.WriteAsync(buffer, 0, read);
                    }

                    var zero = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0));
                    await ns.WriteAsync(zero, 0, 4);
                }

                var reader = new StreamReader(ns, Encoding.ASCII);
                var response = await reader.ReadLineAsync();
                _logger.LogInformation("ClamAV response: {resp}", response);

                return response != null && response.Contains("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Virus scan failed.");
                return false;
            }
        }
    }
}
