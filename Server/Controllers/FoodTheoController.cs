using DominosStockOrder.Server.Models;
using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared;
using DominosStockOrder.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DominosStockOrder.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FoodTheoController : Controller
{
    private readonly StockOrderContext _context;
    private readonly IConsolidatedInventoryService _consolidatedInventoryService;

    public FoodTheoController(StockOrderContext context, IConsolidatedInventoryService consolidatedInventoryService)
    {
        _context = context;
        _consolidatedInventoryService = consolidatedInventoryService;
    }

    // GET
    [HttpGet]
    public IEnumerable<WorkingsVM> Get()
    {
        return _context.InventoryItems.Where(i => !i.ManualCount).Select(i => new { i.Code, i.Description }).Select(desc => new WorkingsVM {
            Description = desc.Description,
            WeeklyFoodTheo = _consolidatedInventoryService.GetItemFoodTheos(desc.Code).ToList()
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
}
