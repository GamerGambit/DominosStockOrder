using DominosStockOrder.Shared.ViewModels;

namespace DominosStockOrder.Client.Services
{
    public interface IInventoryItemService
    {
        Task FetchInventoryDataAsync();

        InventoryItemVM? GetInventoryItemFromPulseCode(string pulseCode);
        Task<InventoryItemVM?> GetInventoryItemFromPulseCodeAsync(string pulseCode);
    }
}
