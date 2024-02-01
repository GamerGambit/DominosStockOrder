using CsvHelper.Configuration.Attributes;

namespace DominosStockOrder.Server.Models.Pulse
{
    /*
    Inventory Item: (1006) Base Gluten Free
    Type: FOOD
    Count Unit: EACH
    Unit Cost: 2.450
    Beginning Inventory: 105.00
    Delivered Quantity: 0.00
    Starting Inventory: 105.00
    Ending Inventory: 0.00
    Actual Usage: 105.00
    Ideal Usage: 0.00
    Actual vs Ideal Usage: 105.00
    Cost Actual Used: $257.23
    Cost vs Ideal: $257.23
    Actual Pct vs. Ideal: 0.00%
    */

    [Delimiter("\t")]
    [CultureInfo("InvariantCulture")]
    public class ConsolidatedInventoryRow
    {
        [Name("Inventory Item")]
        public string ItemNameAndCode { get; set; }

        [Name("Type")]
        public string Type { get; set; }

        [Name("Count Unit")]
        public string Unit { get; set; }

        [Name("Unit Cost")]
        public float UnitCost { get; set; }

        [Name("Beginning Inventory")]
        public float BeginningInventory { get; set; }

        [Name("Delivered Quantity")]
        public float DeliveredQuantity{ get; set; }

        [Name("Starting Inventory")]
        public float StartingInventory { get; set; }

        [Name("Ending Inventory")]
        public float EndingInventory { get; set; }

        [Name("Actual Usage")]
        public float ActualUsage { get; set; }

        [Name("Ideal Usage")]
        public float IdealUsage { get; set; }

        [Name("Actual vs Ideal Usage")]
        public float ActualVsIdeal{ get; set; }

        [Name("Cost Actual Used")]
        public string CostActualUsed { get; set; }

        [Name("Cost vs  Ideal")]
        public string CostVsIdeal { get; set; }

        [Name("Actual Pct vs. Ideal")]
        public string ActualPctVsIdeal {  get;  set; }
    }
}
