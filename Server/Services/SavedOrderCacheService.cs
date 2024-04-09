using DominosStockOrder.Shared.ViewModels;

namespace DominosStockOrder.Server.Services
{
    public class SavedOrderCacheService : ISavedOrderCacheService
    {
        private List<StockOrderResponseItemVM> _savedItems = [];

        public void Clear()
        {
            _savedItems.Clear();
        }

        public IEnumerable<StockOrderResponseItemVM> GetOrderedItems()
        {
            return _savedItems;
        }

        public void SetOrderedItems(IEnumerable<StockOrderResponseItemVM> items)
        {
            _savedItems = items.ToList();
        }

        public bool IsEmpty()
        {
            return _savedItems.Count == 0;
        }
    }
}
