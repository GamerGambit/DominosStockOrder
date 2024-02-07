namespace DominosStockOrder.Shared.ViewModels;

public class InventoryItemVM
{
    public string Code { get; set; }
    public string Description { get; set; }
    public float Multiplier { get; set; }
    public float PackSize { get; set; }
    public bool ManualCount { get; set; }
}