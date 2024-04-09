using DominosStockOrder.Shared.ViewModels;

namespace DominosStockOrder.Server.Services
{
    public interface ISavedOrderCacheService
    {
        void SetOrderedItems(IEnumerable<StockOrderResponseItemVM> items);
        IEnumerable<StockOrderResponseItemVM> GetOrderedItems();
        void Clear();
        bool IsEmpty();
    }
}
