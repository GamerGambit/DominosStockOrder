using DominosStockOrder.Shared.Models.DTOs;

namespace DominosStockOrder.Client.Services;

public interface IInventoryService
{
    DateTime? StartDate { get; set; }
    DateTime? EndDate { get; set; }
    void AddItem(InventoryItemDTO item);

    IEnumerable<InventoryItemDTO> GetItems();

    /// <summary>
    /// Gets the `Ideal Usage` for the supplied inventory code.
    /// </summary>
    /// <param name="code">Pulse inventory item code</param>
    /// <param name="theo">`Ideal Usage` or 0 if code is not found</param>
    /// <returns>true if the item was found, false otherwise</returns>
    bool GetFoodTheoForItemCode(string code, out float theo);

    bool GetCurrentStockForItemCode(string code, out float currentStock);
    bool ShouldImportFoodTheo();
}