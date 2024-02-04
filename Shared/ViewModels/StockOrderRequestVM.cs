namespace DominosStockOrder.Shared.ViewModels;

public class StockOrderRequestVM
{
    public string PurchaseOrderId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public IEnumerable<StockOrderRequestItemVM> Items { get; set; }
}