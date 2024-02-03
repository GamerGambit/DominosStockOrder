using System.Text.Json.Serialization;

namespace DominosStockOrder.Shared.Models.Purchasing
{
    public class OrderResponse
    {
        public string PurchaseOrderId { get; set; }

        [JsonPropertyName("purchaseOrderItems")]
        public IEnumerable<OrderResponseItem> Items { get; set; }
    }
}
