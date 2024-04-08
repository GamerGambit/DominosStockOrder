namespace DominosStockOrder.Client.Models
{
    public class ItemTransfer
    {
        public string PulseCode { get; set; }
        public TransferType TransferType { get; set; }
        public float Quantity { get; set; }
    }
}
