namespace DominosStockOrder.Shared.ViewModels;

public class FoodTheoVM
{
    public DateTime WeekEnding { get; set; }
    public IEnumerable<InventoryItemFoodTheoVM> ItemTheos { get; set; }
}
