namespace DominosStockOrder.Shared.ViewModels;

public class StockOrderRequestItemVM
{
    public string PurchaseOrderItemId { get; set; }
    public string PulseCode { get; set; }
    public string Description { get; set; }
    public int AutoIssue { get; set; }
    public int FinalOrder { get; set; }
    public float PackSize { get; set; }
    public bool IsNewInventory { get; set; }
    public bool IsPackSizeUpdated { get; set; }
    public bool IsItemEnabledRecently { get; set; }
    public bool IsItemCodeChangedRecently { get; set; }
    public StockOrderRequestItemDatabaseInfo? DatabaseInfo { get; set; }
    public float? RollingAverage { get; set; }
    
    // How many weeks do we have average data for?
    public int NumAverageWeeks { get; set; }
}