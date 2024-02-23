namespace DominosStockOrder.Server.Services
{
    public interface IConsolidatedInventoryService
    {
        Task FetchConsolidatedInventoryAsync();
        IList<float> GetItemFoodTheos(string pulseCode);
    }
}
