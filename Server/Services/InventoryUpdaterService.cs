using DominosStockOrder.Server.Models;

using System.Text.RegularExpressions;

namespace DominosStockOrder.Server.Services
{
    public class InventoryUpdaterService : IInventoryUpdaterService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InventoryUpdaterService> _logger;

        private string GetPulseCode(string code)
        {
            return Regex.Replace(code, @"\D+$", string.Empty);
        }

        public InventoryUpdaterService(IServiceProvider serviceProvider, ILogger<InventoryUpdaterService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task AddOrUpdateInventoryItem(string purchaseOrderItemId, string code, string description, float packSize)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StockOrderContext>();

            var pulseCode = GetPulseCode(code);
            var item = await context.InventoryItems.FindAsync(pulseCode);

            // Item exists in database, do nothing
            if (item is not null)
                return;

            _logger.LogInformation("Adding missing inventory item: {portalCode} => {pulseCode}", code, pulseCode);
            await context.InventoryItems.AddAsync(new InventoryItem
            {
                Code = pulseCode,
                Description = description,
                PackSize = packSize,
                Multiplier = 1
            });

            await context.SaveChangesAsync();
        }
    }
}
