using DominosStockOrder.Server.Models;
using DominosStockOrder.Server.PulseApi;
using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared.ViewModels;

using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace DominosStockOrder.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FoodTheoController : Controller
{
    private readonly StockOrderContext _context;
    private readonly IConsolidatedInventoryService _consolidatedInventoryService;
    private readonly IPulseApiClient _pulseApiClient;

    public FoodTheoController(StockOrderContext context, IConsolidatedInventoryService consolidatedInventoryService, IPulseApiClient pulseApiClient)
    {
        _context = context;
        _consolidatedInventoryService = consolidatedInventoryService;
        _pulseApiClient = pulseApiClient;
    }

    // GET
    [HttpGet]
    public IEnumerable<WorkingsVM> Get()
    {
        var itemDatas = _context.InventoryItems.Where(i => !i.ManualCount).Select(i => new { i.Code, i.IgnoreFoodTheoBefore }).ToArray();
        
        return itemDatas.Select(data => new WorkingsVM
        {
            PulseCode = data.Code,
            WeeklyFoodTheo = _consolidatedInventoryService.GetItemFoodTheos(data.Code).Where(x => data.IgnoreFoodTheoBefore == null || x.WeekEnding >= data.IgnoreFoodTheoBefore).ToList(),
            EndingInventory = _consolidatedInventoryService.GetItemEndingInventory(data.Code),
        });
    }

    // POST
    [HttpPut("initial/{pulseCode}")]
    public async Task<IActionResult> PutInitial(string pulseCode, [FromBody] float initialWeeklyTheo)
    {
        var item = await _context.InventoryItems.FindAsync(pulseCode);

        if (item is null)
            return NotFound();

        item.InitialFoodTheo = initialWeeklyTheo;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("extra/{date}")]
    public async Task<IEnumerable<ExtraInventoryVM>> GetExtra(DateTime date)
    {
        var ret = new List<ExtraInventoryVM>();

        var inventories = await _pulseApiClient.ConsolidatedInventoryAsync(date, date);

        foreach (var inv in inventories)
        {
            var match = Regex.Match(inv.Description, @"^\(([\d\w]+)\) (.*)$");
            var code = match.Groups[1].Value;

            // Failed to match description so skip it. Its probably not useful to us anyway
            if (!match.Success)
                continue;

            ret.Add(new ExtraInventoryVM
            {
                PulseCode = code,
                FoodTheo = inv.IdealUsage
            });
        }

        return ret;
    }

    [HttpPost("custom")]
    public async Task<IEnumerable<FoodTheoVM>> GetCustom([FromBody] IEnumerable<DateRange> dateRanges)
    {
        var data = new List<FoodTheoVM>();

        foreach (var range in dateRanges)
        {
            var inventories = await _pulseApiClient.ConsolidatedInventoryAsync(range.Start, range.End);
            var vmInventories = new List<InventoryItemFoodTheoVM>();

            foreach (var inv in inventories)
            {
                var match = Regex.Match(inv.Description, @"^\(([\d\w]+)\) (.*)$");
                var code = match.Groups[1].Value;

                vmInventories.Add(new InventoryItemFoodTheoVM
                {
                    PulseCode = code,
                    IdealUsage = inv.IdealUsage
                });
            }

            data.Add(new FoodTheoVM
            {
                WeekEnding = range.End,
                ItemTheos = vmInventories
            });
        }

        return data;
    }
}
