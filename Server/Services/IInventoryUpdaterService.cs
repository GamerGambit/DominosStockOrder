namespace DominosStockOrder.Server.Services
{
    public interface IInventoryUpdaterService
    {
        Task AddOrUpdateInventoryItem(string purchaseOrderItemId, string code, float packSize);
    }
}
