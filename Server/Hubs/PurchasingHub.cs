using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared.Models.Purchasing;

using Microsoft.AspNetCore.SignalR;

namespace DominosStockOrder.Server.Hubs
{
    public class PurchasingHub : Hub<IPurchasingClient>
    {
        public async Task SetPendingOrders(IEnumerable<OrderRequest> orders, IInventoryUpdaterService inventoryUpdater, IPendingOrdersCacheService pendingOrdersCache)
        {
            pendingOrdersCache.ClearOrders();

            if (!orders.Any())
                return;

            // ignore other orders
            var order = orders.First();

            pendingOrdersCache.AddOrder(order);

            foreach (var item in order.Items)
            {
                await inventoryUpdater.AddOrUpdateInventoryItem(item.PurchaseOrderItemId, item.Code, item.PackSizeQuantity);
            }
        }

        public async Task PlaceOrder(OrderResponse order)
        {
            await Clients.All.PlaceOrder(order);
        }
    }
}
