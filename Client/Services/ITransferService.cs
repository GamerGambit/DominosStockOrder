using DominosStockOrder.Client.Models;

namespace DominosStockOrder.Client.Services
{
    public interface ITransferService
    {
        ItemTransfer? GetTransferForPulseCode(string pulseCode);
        void AddOrSetTransfer(ItemTransfer itemTransfer);
        void RemoveTransferForPulseCode(string pulseCode);
        IEnumerable<ItemTransfer> GetAllTransfers();
    }
}
