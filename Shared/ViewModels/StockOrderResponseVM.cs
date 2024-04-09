namespace DominosStockOrder.Shared.ViewModels
{
    public class StockOrderResponseVM
    {
        public string PurchaseOrderId { get; set; }
        public IEnumerable<StockOrderResponseItemVM> Items { get; set; }
    }
}
