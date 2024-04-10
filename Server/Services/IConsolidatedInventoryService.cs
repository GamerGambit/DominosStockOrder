using DominosStockOrder.Shared.DTOs;

namespace DominosStockOrder.Server.Services
{
    public interface IConsolidatedInventoryService
    {
        Task FetchWeeklyFoodTheoAsync();
        Task FetchEndingInventoryAsync(DateTime date);
        IList<ItemWeeklyFoodTheo> GetItemFoodTheos(string pulseCode);
        float GetItemEndingInventory(string pulseCode);
    }
}
