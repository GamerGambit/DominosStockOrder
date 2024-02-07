using DominosStockOrder.Server.Models;
using DominosStockOrder.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DominosStockOrder.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    private readonly StockOrderContext _context;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(StockOrderContext context, ILogger<SettingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<InventoryItemVM> Get()
    {
        return _context.InventoryItems.Select(i => new InventoryItemVM
        {
            Code = i.Code,
            Description = i.Description,
            Multiplier = i.Multiplier,
            PackSize = i.PackSize,
            ManualCount = i.ManualCount
        });
    }

    [HttpPut]
    public async Task<IActionResult> Put(IEnumerable<InventoryItemVM> items)
    {
        foreach (var item in items)
        {
            var entry = await _context.InventoryItems.FindAsync(item.Code);

            if (entry is null)
            {
                _logger.LogError("PUT: Failed to find item with code {code}", item.Code);
                continue;
            }

            entry.PackSize = item.PackSize;
            entry.ManualCount = item.ManualCount;
            entry.Multiplier = item.Multiplier;
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
}