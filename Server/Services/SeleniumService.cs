using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal.Logging;

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
            options.AddArgument("--no-sandbox"); // this is a hack to work around chrome running as root. https://stackoverflow.com/a/70468050
            options.AddArgument("--headless");
            options.AddArgument("--remote-debugging-port=9222"); // Default debug port, just make sure as its bound from the docker container
            options.AddArgument("--remote-debugging-address=0.0.0.0");
            options.AddArgument("--auto-open-devtools-for-tabs"); // Always do this for easier debugging

            var service = ChromeDriverService.CreateDefaultService(/*"./chromedriver"*/);
            service.AllowedIPAddresses = " "; // allow anyone to debug
            _driver = new ChromeDriver(service, options);

            ///@TODO bail if these env-vars arent set as we cant login or get orders from the portal
            var envStoreId = Environment.GetEnvironmentVariable("PORTAL_STOREID");
            var envUser = Environment.GetEnvironmentVariable("PORTAL_USER");
            var envPass = Environment.GetEnvironmentVariable("PORTAL_PASS");
            var fixedHost = host.Replace("*", "localhost"); // the host may report "http://*:8080" depending on configuration
            var source = $"const storeId = \"{envStoreId}\";\nconst username = \"{envUser}\";\nconst password = \"{envPass}\";\nconst stockServer = \"{fixedHost}\";\n";
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
