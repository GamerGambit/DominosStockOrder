using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DominosStockOrder.Server.Models;
using DominosStockOrder.Shared.Models;
using Microsoft.Data.Sqlite;

namespace DominosStockOrder.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryItemsController : ControllerBase
    {
        private readonly StockOrderContext _context;

        public InventoryItemsController(StockOrderContext context)
        {
            _context = context;
        }

        // GET: api/InventoryItems
        [HttpGet]
        public IEnumerable<InventoryItem> Get()
        {
            return _context.InventoryItems.AsEnumerable();
        }

        // GET: api/InventoryItems/{pulseCode}
        [HttpGet("{pulseCode}")]
        public async Task<ActionResult<InventoryItem?>> Get(string pulseCode)
        {
            var entry = await _context.InventoryItems.FindAsync(pulseCode);

            if (entry is null)
                return NotFound();

            return Ok(entry);
        }
    }
}
