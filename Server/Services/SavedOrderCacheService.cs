using DominosStockOrder.Shared.ViewModels;

namespace DominosStockOrder.Server.Services
{
    public class SavedOrderCacheService : ISavedOrderCacheService
    {
        private StockOrderResponseVM? _savedOrder = null;

        public void Clear()
        {
            _savedOrder = null;
        }

        public StockOrderResponseVM GetSavedOrder()
        {
            if (_savedOrder == null)
                throw new InvalidOperationException("No saved order");

            return _savedOrder;
        }

        public void SetSavedOrder(StockOrderResponseVM order)
        {
            _savedOrder = order;
        }

        public bool HasSavedOrder()
        {
            return _savedOrder != null;
        }
    }
}
