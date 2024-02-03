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

        public async Task AddOrUpdateInventoryItem(string purchaseOrderItemId, string code, float packSize)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StockOrderContext>();

            var pulseCode = GetPulseCode(code);
            var item = context.InventoryItems.Find(pulseCode);

            if (item is null)
            {
                _logger.LogInformation("Adding missing inventory item: {portalCode} => {pulseCode}", code, pulseCode);
                await context.InventoryItems.AddAsync(new InventoryItem
                {
                    Code = pulseCode,
                    PortalItemId = purchaseOrderItemId,
                    PackSize = packSize
                });

                await context.SaveChangesAsync();
            }
            else
            {
                if (item.PortalItemId is null)
                {
                    _logger.LogInformation("Adding missing portal code to inventory item: {pulseCode} => {portalId}", pulseCode, purchaseOrderItemId);
                    item.PortalItemId = purchaseOrderItemId;
                    await context.SaveChangesAsync();
                }
                else if (item.PortalItemId != purchaseOrderItemId)
                {
                    _logger.LogCritical("Inventory item mismatch: {pulseCode} has {currentCode} but portal has {portalId}", pulseCode, item.PortalItemId, purchaseOrderItemId);
                }
            }
        }
    }
}
