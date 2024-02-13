using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DominosStockOrder.Server.Models;

[PrimaryKey(nameof(ItemCode), nameof(WeekEnding))]
public class ItemAverages
{
    public string ItemCode { get; set; }
    public DateTime WeekEnding { get; set; }
    public float FoodTheo { get; set; }
    
    [ForeignKey("ItemCode")]
    public InventoryItem Item { get; set; }
}