namespace DominosStockOrder.Client.Services
{
    public interface IExtraInventoryService
    {
        Task FetchExtraInventoryFor(DateTime date);
        float GetExtraInventoryForPulseCode(string pulseCode);
        void Clear();
    }
}
