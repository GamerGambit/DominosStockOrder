namespace DominosStockOrder.Shared.Models.Purchasing
{
    public class OrderRequest
    {
        public string PurchaseOrderId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public IEnumerable<OrderRequestItem> Items { get; set; }
    }
}
