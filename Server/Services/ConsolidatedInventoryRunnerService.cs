using DominosStockOrder.Server.PulseApi;

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

        public ConsolidatedInventoryRunnerService(IServiceProvider serviceProvider, IConsolidatedInventoryService consolidatedInventoryService)
        {
            _processingDate = DateTime.Now;
            _serviceProvider = serviceProvider;
            _consolidatedInventoryService = consolidatedInventoryService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceProvider.CreateScope();
            var pulse = scope.ServiceProvider.GetRequiredService<IPulseApiClient>();

            while (! await pulse.CheckEODAsync(_processingDate, stoppingToken))
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            await _consolidatedInventoryService.FetchConsolidatedInventoryAsync();
        }
    }
}
