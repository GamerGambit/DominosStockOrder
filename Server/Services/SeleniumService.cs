using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using OpenQA.Selenium.Chrome;

namespace DominosStockOrder.Server.Services
{
    public class SeleniumService : BackgroundService
    {
        private readonly IServer _server;
        private ChromeDriver? _driver;

        public SeleniumService(IServer server)
        {
            _server = server;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await PreStartBrowser(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();
            }

            _driver?.Quit();
        }

        private async Task PreStartBrowser(CancellationToken cancellationToken)
        {
            var addresses = _server.Features.Get<IServerAddressesFeature>().Addresses;

            while (!cancellationToken.IsCancellationRequested && addresses.Count == 0)
            {
                await Task.Yield();
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            StartBrowser(addresses.FirstOrDefault(string.Empty));
        }

        private void StartBrowser(string host)
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--remote-debugging-port=9222"); // Default debug port, just make sure as its bound from the docker container
            options.AddArgument("--incognito");
            options.AddArgument("--auto-open-devtools-for-tabs"); // Always do this for easier debugging
            _driver = new ChromeDriver(options);

            var envStoreId = Environment.GetEnvironmentVariable("PORTAL_STOREID");
            var envUser = Environment.GetEnvironmentVariable("PORTAL_USER");
            var envPass = Environment.GetEnvironmentVariable("PORTAL_PASS");
            var source = $"const storeId = \"{envStoreId}\"; const username = \"{envUser}\"; const password = \"{envPass}\"; const stockServer = \"{host}\";";
            source += File.ReadAllText("DPEPurchasingMarshaller.js");

            var cmdparams = new Dictionary<string, object>
            {
                { "source", source }
            };
            _driver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", cmdparams);

            _driver.Navigate().GoToUrl("https://purchasing.dominos.com.au");
        }
    }
}
