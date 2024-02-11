namespace DominosStockOrder.Shared.Models.DTOs
{
    public class InventoryItemDTO
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public float EndingInventory { get; set; }
        public float IdealUsage { get; set; }
    }
}
