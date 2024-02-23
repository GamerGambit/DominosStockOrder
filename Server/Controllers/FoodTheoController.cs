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
