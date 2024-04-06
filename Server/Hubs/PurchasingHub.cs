using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared;
using DominosStockOrder.Shared.Models.Purchasing;

using Microsoft.AspNetCore.SignalR;

namespace DominosStockOrder.Server.Hubs
{
    public class PurchasingHub : Hub<IPurchasingClient>
    {
        private int _numClients = 0;
        private readonly Status _statusService;
        private readonly ILogger<PurchasingHub> _logger;

        public PurchasingHub(Status statusService, ILogger<PurchasingHub> logger)
        {
            _statusService = statusService;
            _logger = logger;
        }

        public async Task SetPendingOrders(IEnumerable<OrderRequest> orders, IInventoryUpdaterService inventoryUpdater, IPendingOrdersCacheService pendingOrdersCache)
        {
            pendingOrdersCache.ClearOrders();

            if (!orders.Any())
                return;

            // ignore other orders
            var order = orders.First();

            pendingOrdersCache.AddOrder(order);

            _statusService.EODRun = order.DeliveryDate.Date == DateTime.Now.Date;
            _logger.LogInformation("Received order for {date}, setting EODRun to {eod}", order.DeliveryDate.ToString("d"), _statusService.EODRun);

            foreach (var item in order.Items)
            {
                await inventoryUpdater.AddOrUpdateInventoryItem(item.PurchaseOrderItemId, item.Code, item.Description, item.PackSizeQuantity);
            }
        }

        public async Task PlaceOrder(OrderResponse order)
        {
            await Clients.All.PlaceOrder(order);
        }

        public void OrderSuccessful()
        {
            _statusService.IsOrderSuccessful = true;
            _statusService.OrderError = string.Empty;
        }

        public void OrderFailed(string error)
        {
            _statusService.IsOrderSuccessful = false;
            _statusService.OrderError = error;
        }

        public override Task OnConnectedAsync()
        {
            ++_numClients;
            _statusService.IsConnectedToBrowser = true;
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            --_numClients;
            _statusService.IsConnectedToBrowser = _numClients > 0;
            return Task.CompletedTask;
        }
    }
}
