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
        return _context.InventoryItems.Where(i => !i.ManualCount).Select(i => i.Code).Select(code => new WorkingsVM {
            PulseCode = code,
            WeeklyFoodTheo = _consolidatedInventoryService.GetItemFoodTheos(code).ToList(),
            EndingInventory = _consolidatedInventoryService.GetItemEndingInventory(code)
        });
    }

    // POST
    [HttpPut("initial/{pulseCode}")]
    public async Task<IActionResult> PutInitial(string pulseCode, [FromBody] float initialWeeklyTheo)
    {
        var entry = _context.InitialFoodTheos.Where(e => e.PulseCode == pulseCode).FirstOrDefault();

        if (entry is not null)
            return Conflict();

        _context.InitialFoodTheos.Add(new ItemInitialFoodTheo
        {
            PulseCode = pulseCode,
            InitialFoodTheo = initialWeeklyTheo
        });

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

            ret.Add(new ExtraInventoryVM
            {
                PulseCode = code,
                FoodTheo = inv.EndingInventory
            });
        }

        return ret;
    }
}
