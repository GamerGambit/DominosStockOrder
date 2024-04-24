using DominosStockOrder.Shared.ViewModels;

namespace DominosStockOrder.Server.Services
{
    public interface ISavedOrderCacheService
    {
        void SetSavedOrder(StockOrderResponseVM order);
        StockOrderResponseVM GetSavedOrder();
        void Clear();
        bool HasSavedOrder();
    }
}
