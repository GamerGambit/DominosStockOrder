using DominosStockOrder.Shared.Models.Purchasing;

namespace DominosStockOrder.Server.Services
{
    public class PendingOrdersCacheService : IPendingOrdersCacheService
    {
        private readonly List<OrderRequest> _orders = [];

        public IEnumerable<OrderRequest> GetOrders() => _orders;

        public void AddOrder(OrderRequest order)
        {
            _orders.Add(order);
        }

        public void ClearOrders()
        {
            _orders.Clear();
        }
    }
}
