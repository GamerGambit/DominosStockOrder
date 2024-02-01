using System.ComponentModel.DataAnnotations;

namespace DominosStockOrder.Server.Models
{
    public class StockOrder
    {
        [Key]
        public string PortalOrderId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public IEnumerable<InventoryItem> Items { get; set; }
    }
}
