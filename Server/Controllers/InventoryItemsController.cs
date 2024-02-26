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

        // POST: api/InventoryItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IEnumerable<int>> PostInventoryItem(IEnumerable<string> itemCodes)
        {
            var statusCodes = new List<int>();

            foreach (var code in itemCodes)
            {

                if (InventoryItemExists(code))
                {
                    statusCodes.Add(Conflict().StatusCode);
                }
                else
                {
                    _context.InventoryItems.Add(new InventoryItem
                    {
                        Code = code,
                        PortalItemId = null,
                        PackSize = 0,
                        Multiplier = 1,
                        ManualCount = false
                    });

                    await _context.SaveChangesAsync();
                    statusCodes.Add(Created().StatusCode.Value);
                }
            }

            return statusCodes;
        }

        private bool InventoryItemExists(string id)
        {
            return _context.InventoryItems.Any(e => e.Code == id);
        }
    }
}
