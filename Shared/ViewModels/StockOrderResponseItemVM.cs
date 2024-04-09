namespace DominosStockOrder.Shared.ViewModels
{
    public class StockOrderResponseItemVM
    {
        public string PurchaseOrderItemId { get; set; }
        public string PulseCode { get; set; }
        public int? Override { get; set; }
        public float? Transferred { get; set; }
        public float ExtraIdeal { get; set; }
        public float InTransit { get; set; }
    }
}
