using DominosStockOrder.Shared.Models.Purchasing;

using Microsoft.AspNetCore.SignalR;

namespace DominosStockOrder.Server.Hubs
{
    public class PurchasingHub : Hub
    {
        public async Task SetPendingOrders(IEnumerable<OrderRequest> orders)
        {
            //
        }

        public async Task PlaceOrder(OrderResponse order)
        {
            await Clients.All.SendAsync("PlaceOrder", order);
        }
    }
}
