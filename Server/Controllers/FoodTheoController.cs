using DominosStockOrder.Server.Models;
using DominosStockOrder.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DominosStockOrder.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FoodTheoController : Controller
{
    private const int NumWeeks = 8; // How many weeks should we track?
    private readonly StockOrderContext _context;

    public FoodTheoController(StockOrderContext context)
    {
        _context = context;
    }

    // PUT
    [HttpPut]
    public async Task<IActionResult> Put(FoodTheoVM foodTheoVm)
    {
        // Only worry about inventory items that the portal has tracked
        var trimmed = foodTheoVm.ItemTheos.Where(theoVm => _context.InventoryItems.Find(theoVm.PulseCode) is not null);
        var averages = trimmed.Select(theoVm => new ItemAverages
        {
            ItemCode = theoVm.PulseCode,
            WeekEnding = foodTheoVm.WeekEnding,
            FoodTheo = theoVm.IdealUsage
        });

        _context.ItemAverages.AddRange(averages);

        PruneOldFoodTheo();

        await _context.SaveChangesAsync();

        return Ok();
    }

    private void PruneOldFoodTheo()
    {
        var pruneList = _context.ItemAverages.GroupBy(ia => ia.ItemCode).Where(g => g.Count() > NumWeeks)
            .Select(g => g.OrderBy(ia => ia.WeekEnding).First()).ToList();

        _context.RemoveRange(pruneList);
    }
}
