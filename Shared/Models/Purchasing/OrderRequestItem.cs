namespace DominosStockOrder.Shared.Models.Purchasing
{
    public class OrderRequestItem
    {
        public string PurchaseOrderItemId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public float PackSizeQuantity { get; set; }
        public int AutoIssue { get; set; }
        public int? Override { get; set; }
        public int FinalOrder { get; set; }
        public bool IsNewInventory { get; set; }
        public bool IsPacksizeupdated { get; set; }
        public bool IsItemEnabledRecently { get; set; }
        public bool IsItemCodeChangedRecently { get; set; }
    }
}