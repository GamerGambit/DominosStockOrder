using DominosStockOrder.Client.Models;

namespace DominosStockOrder.Client.Services
{
    public class TransferService : ITransferService
    {
        private readonly Dictionary<string, ItemTransfer> _items = [];
        public IEnumerable<ItemTransfer> GetAllTransfers() => _items.Values;

        public ItemTransfer? GetTransferForPulseCode(string pulseCode)
        {
            if (_items.TryGetValue(pulseCode, out var qty))
                return qty;

            return null;
        }

        public void AddOrSetTransfer(ItemTransfer itemTransfer)
        {
            if (string.IsNullOrWhiteSpace(itemTransfer.PulseCode))
                throw new ArgumentException("Invalid PulseCode", nameof(itemTransfer));

            if (!_items.TryAdd(itemTransfer.PulseCode, itemTransfer))
            {
                _items[itemTransfer.PulseCode] = itemTransfer;
            }
        }

        public void RemoveTransferForPulseCode(string pulseCode)
        {
            _items.Remove(pulseCode);
        }
    }
}
