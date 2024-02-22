
using System.Diagnostics;

namespace DominosStockOrder.Server.Services
{
    /// <summary>
    /// Starts the Firefox browser.
    /// The browser should have a Userscript with SignalR setup to communicate with the server.
    /// </summary>
    public class FirefoxService : IHostedService
    {
        private Process? _process;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/firefox",
                    Arguments = "-headless -P stockorder https://purchasing.dominos.com.au",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };

            _process.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_process != null)
            {
                _process.Kill();
                _process.Dispose();
                _process = null;
            }

            return Task.CompletedTask;
        }
    }
}
