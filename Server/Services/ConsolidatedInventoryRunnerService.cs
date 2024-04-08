using DominosStockOrder.Server.PulseApi;
using DominosStockOrder.Shared;

namespace DominosStockOrder.Server.Services
{
    /// <summary>
    /// This service is responsible for checking if end-of-day has been run
    /// and if so, will fetch consolidated inventory data
    /// </summary>
    public class ConsolidatedInventoryRunnerService : BackgroundService
    {
        private readonly DateTime _processingDate;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConsolidatedInventoryService _consolidatedInventoryService;
        private readonly ILogger<ConsolidatedInventoryRunnerService> _logger;
        private readonly Status _statusService;

        public ConsolidatedInventoryRunnerService(IServiceProvider serviceProvider, IConsolidatedInventoryService consolidatedInventoryService, ILogger<ConsolidatedInventoryRunnerService> logger, Status statusService)
        {
            var now = DateTime.Now;
            if (now.Hour < 6)
            {
                _processingDate = now.AddDays(-1);
            }
            else
            {
                _processingDate = now;
            }

            _serviceProvider = serviceProvider;
            _consolidatedInventoryService = consolidatedInventoryService;
            _logger = logger;
            _statusService = statusService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceProvider.CreateScope();
            var pulse = scope.ServiceProvider.GetRequiredService<IPulseApiClient>();

            _logger.LogInformation("Checking EOD for {processingDate}", _processingDate.ToString("d"));

            while (! await pulse.CheckEODAsync(_processingDate, stoppingToken))
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _statusService.EODRun = true;

            _logger.LogInformation("EOD run at {now}, pulling weekly food", DateTime.Now.ToString());
            await _consolidatedInventoryService.FetchWeeklyFoodTheoAsync();

            _logger.LogInformation("Pulling ending inventory for {processingDate}", _processingDate.ToString("d"));
            await _consolidatedInventoryService.FetchEndingInventoryAsync(_processingDate);

            _logger.LogInformation("Service terminated at {now}", DateTime.Now.ToString());
        }
    }
}
