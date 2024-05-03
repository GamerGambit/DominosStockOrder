using DominosStockOrder.Server.Models;
using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DominosStockOrder.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SettingsController : ControllerBase
{
    private readonly StockOrderContext _context;
    private readonly ILogger<SettingsController> _logger;
    private readonly IConsolidatedInventoryService _consolidatedInventory;

    public SettingsController(StockOrderContext context, ILogger<SettingsController> logger, IConsolidatedInventoryService consolidatedInventory)
    {
        _context = context;
        _logger = logger;
        _consolidatedInventory = consolidatedInventory;
    }

    [HttpGet]
    public async Task<SettingsVM> Get()
    {
        var settings = _context.Settings.First();

        return new SettingsVM
        {
            NumFoodTheoWeeks = settings.NumFoodTheoWeeks,
        };
    }

    [HttpPut]
    public async Task Put(SettingsVM settingsVM)
    {
        var settings = _context.Settings.First();

        settings.NumFoodTheoWeeks = settingsVM.NumFoodTheoWeeks;

        await _context.SaveChangesAsync();
    }

    [HttpGet("Items")]
    public IEnumerable<InventoryItemVM> GetItems()
    {
        return _context.InventoryItems.Select(i => new InventoryItemVM
        {
            Code = i.Code,
            Description = i.Description,
            Multiplier = i.Multiplier,
            PackSize = i.PackSize,
            ManualCount = i.ManualCount,
            DoubleCheck = i.DoubleCheck,
            Comment = i.Comment,
            InitialFoodTheo = i.InitialFoodTheo,
            IgnoreFoodTheoBefore = i.IgnoreFoodTheoBefore
        });
    }

    [HttpPut("Items")]
    public async Task<IActionResult> PutItems(IEnumerable<InventoryItemVM> items)
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
            entry.DoubleCheck = item.DoubleCheck;
            entry.Comment = item.Comment;
            entry.InitialFoodTheo = item.InitialFoodTheo;
            entry.IgnoreFoodTheoBefore = item.IgnoreFoodTheoBefore;
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
}