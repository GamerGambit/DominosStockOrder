using DominosStockOrder.Shared.ViewModels;

namespace DominosStockOrder.Client.Services
{
    public interface IInventoryItemService
    {
        Task FetchInventoryDataAsync();
        Task<InventoryItemVM?> GetInventoryItemFromPulseCodeAsync(string pulseCode);
    }
}
