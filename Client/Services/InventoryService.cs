using DominosStockOrder.Shared.Models.DTOs;

namespace DominosStockOrder.Client.Services;

public class InventoryService : IInventoryService
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<InventoryItemDTO> Items = [];

    public void AddItem(InventoryItemDTO item)
    {
        Items.Add(item);
    }

    /// <summary>
    /// Gets the `Ideal Usage` for the supplied inventory code.
    /// </summary>
    /// <param name="code">Pulse inventory item code</param>
    /// <param name="theo">`Ideal Usage` or 0 if code is not found</param>
    /// <returns>true if the item was found, false otherwise</returns>
    public bool GetFoodTheoForItemCode(string code, out float theo)
    {
        var found = Items.Find(i => i.Code == code);

        if (found is null)
        {
            theo = 0;
            return false;
        }

        theo = found.IdealUsage;
        return true;
    }

    public bool GetCurrentStockForItemCode(string code, out float currentStock)
    {
        var found = Items.Find(i => i.Code == code);

        if (found is null)
        {
            currentStock = 0;
            return false;
        }

        currentStock = found.EndingInventory;
        return true;
    }

    public bool ShouldImportFoodTheo()
    {
        if (StartDate is null || EndDate is null)
            return false;
        
        // Only import food theo on a full week report (mon - sun) and make sure its for 1 week
        return StartDate.Value.DayOfWeek == DayOfWeek.Monday && EndDate.Value.DayOfWeek == DayOfWeek.Sunday && (int)(EndDate - StartDate).Value.TotalDays == 6;
    }
}