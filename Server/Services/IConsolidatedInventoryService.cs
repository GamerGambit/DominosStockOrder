namespace DominosStockOrder.Server.Services
{
    public interface IConsolidatedInventoryService
    {
        Task FetchWeeklyFoodTheoAsync();
        Task FetchEndingInventoryAsync(DateTime date);
        IList<float> GetItemFoodTheos(string pulseCode);
        float GetItemEndingInventory(string pulseCode);
    }
}
