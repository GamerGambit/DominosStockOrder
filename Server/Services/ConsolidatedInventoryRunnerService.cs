﻿using DominosStockOrder.Server.PulseApi;

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

        public ConsolidatedInventoryRunnerService(IServiceProvider serviceProvider, IConsolidatedInventoryService consolidatedInventoryService, ILogger<ConsolidatedInventoryRunnerService> logger)
        {
            _processingDate = DateTime.Now;
            _serviceProvider = serviceProvider;
            _consolidatedInventoryService = consolidatedInventoryService;
            _logger = logger;
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

            _logger.LogInformation("EOD run at {now}, pulling weekly food", DateTime.Now.ToString());
            await _consolidatedInventoryService.FetchWeeklyFoodTheoAsync();

            _logger.LogInformation("Pulling ending inventory for {processingDate}", _processingDate.ToString("d"));
            await _consolidatedInventoryService.FetchEndingInventoryAsync(_processingDate);

            _logger.LogInformation("Service terminated at {now}", DateTime.Now.ToString());
        }
    }
}
