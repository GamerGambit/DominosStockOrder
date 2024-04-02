namespace DominosStockOrder.Shared.ViewModels;

public class StockOrderRequestItemVM
{
    public string PurchaseOrderItemId { get; set; }
    public string PulseCode { get; set; }
    public string Description { get; set; }
    public int Suggested { get; set; }
    public int AutoIssue { get; set; }
    public int? Override { get; set; }
    public float PackSize { get; set; }
    public int InTransit { get; set; }
    public bool IsNewInventory { get; set; }
    public bool IsPackSizeUpdated { get; set; }
    public bool IsItemEnabledRecently { get; set; }
    public bool IsItemCodeChangedRecently { get; set; }
}