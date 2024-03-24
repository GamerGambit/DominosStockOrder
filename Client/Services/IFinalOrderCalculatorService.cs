namespace DominosStockOrder.Client.Services
{
    public interface IFinalOrderCalculatorService
    {
        Task<int> CalculateFinalOrder(string pulseCode, int inTransit);
    }
}
