using DominosStockOrder.Server.Models;
using DominosStockOrder.Shared;
using DominosStockOrder.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DominosStockOrder.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FoodTheoController : Controller
{
    private readonly StockOrderContext _context;

    public FoodTheoController(StockOrderContext context)
    {
        _context = context;
    }

    // PUT
    [HttpPut("initial/{pulseCode}")]
    public async Task PutInitial(string pulseCode, [FromBody] float initialWeeklyTheo)
    {
        var avgs = _context.ItemAverages.Where(ia => ia.ItemCode == pulseCode).Select(ia => ia.WeekEnding).OrderBy(we => we).ToList();
        var diff = Constants.NumFoodTheoWeeks - avgs.Count;

        var useWkEnding = avgs.Any() ? avgs.First() : DateTime.Now;

        for (int i = 0; i < diff; ++i)
        {
            useWkEnding -= TimeSpan.FromDays(6); // go back 1 week each time
            _context.ItemAverages.Add(new ItemAverages
            {
                ItemCode = pulseCode,
                WeekEnding = useWkEnding,
                FoodTheo = initialWeeklyTheo
            });
        }

        await _context.SaveChangesAsync();
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
        var pruneList = _context.ItemAverages.GroupBy(ia => ia.ItemCode).Where(g => g.Count() > Constants.NumFoodTheoWeeks)
            .Select(g => g.OrderBy(ia => ia.WeekEnding).First()).ToList();

        _context.RemoveRange(pruneList);
    }
}
