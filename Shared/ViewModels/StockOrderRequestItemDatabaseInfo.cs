namespace DominosStockOrder.Shared.ViewModels;

public class StockOrderRequestItemDatabaseInfo
{
    public string Code { get; set; }
    public float PackSize { get; set; }
    public float Multiplier { get; set; }
    public bool ManualCount { get; set; }
}