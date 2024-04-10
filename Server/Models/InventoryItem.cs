using System.ComponentModel.DataAnnotations;

namespace DominosStockOrder.Server.Models
{
    public class InventoryItem
    {
        [Key]
        public required string Code { get; set; }
        public string Description { get; set; }
        public float PackSize { get; set; }
        public float Multiplier { get; set; }
        public bool ManualCount { get; set; }
        public bool DoubleCheck { get; set; }
        public string? Comment { get; set; }

        /// <summary>
        /// Projected weekly food theo
        /// </summary>
        public float? InitialFoodTheo { get; set; }

        /// <summary>
        /// When set, ignore all food usage from consolidated inventory before this date.
        /// This effectively resets the food usage and <paramref name="InitialFoodTheo"></paramref> will be substituted for missing weeks
        /// </summary>
        public DateTime? IgnoreFoodTheoBefore { get; set; }
    }
}
