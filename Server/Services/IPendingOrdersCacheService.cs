using DominosStockOrder.Shared.Models.Purchasing;

namespace DominosStockOrder.Server.Services
{
    public interface IPendingOrdersCacheService
    {
        IEnumerable<OrderRequest> GetOrders();
        void AddOrder(OrderRequest order);
        void ClearOrders();
        bool HasPendingOrder();
    }
}
