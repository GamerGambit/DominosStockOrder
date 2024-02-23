using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominosStockOrder.Server.Models
{
    /// <summary>
    /// Model used to fill gaps in food theo (Used primarily for new items)
    /// </summary>
    public class ItemInitialFoodTheo
    {
        /// <summary>
        /// Inventory Item Pulse code
        /// </summary>
        [Key]
        public string PulseCode { get; set; }

        /// <summary>
        /// Projected weekly food theo
        /// </summary>
        public float InitialFoodTheo { get; set; }

        [ForeignKey("PulseCode")]
        public InventoryItem Item { get; set; }
    }
}
