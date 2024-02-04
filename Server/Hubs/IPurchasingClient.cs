using DominosStockOrder.Shared.Models.Purchasing;

namespace DominosStockOrder.Server.Hubs;

public interface IPurchasingClient
{
    Task PlaceOrder(OrderResponse order);
}