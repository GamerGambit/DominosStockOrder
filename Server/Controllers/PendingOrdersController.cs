using System.Text.RegularExpressions;
using DominosStockOrder.Server.Hubs;
using DominosStockOrder.Server.Models;
using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared.Models.Purchasing;
using DominosStockOrder.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DominosStockOrder.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingOrdersController : ControllerBase
    {
        private readonly IPendingOrdersCacheService _pendingOrdersCache;
        private readonly StockOrderContext _context;
        private readonly IHubContext<PurchasingHub, IPurchasingClient> _hub;
        private readonly IConsolidatedInventoryService _consolidatedInventory;

        public PendingOrdersController(IPendingOrdersCacheService pendingOrdersCache, StockOrderContext context, IHubContext<PurchasingHub, IPurchasingClient> hub, IConsolidatedInventoryService consolidatedInventory)
        {
            _pendingOrdersCache = pendingOrdersCache;
            _context = context;
            _hub = hub;
            _consolidatedInventory = consolidatedInventory;
        }

        // GET: api/<PendingOrdersController>
        [HttpGet]
        public IEnumerable<StockOrderRequestVM> Get()
        {
            return _pendingOrdersCache.GetOrders().Select(o => new StockOrderRequestVM
            {
                PurchaseOrderId = o.PurchaseOrderId,
                DeliveryDate = o.DeliveryDate,
                Items = o.Items.Select(async i =>
                {
                    var pulseCode = Regex.Replace(i.Code, @"\D+$", string.Empty);
                    var averages = _consolidatedInventory.GetItemFoodTheos(pulseCode);
                    return new StockOrderRequestItemVM
                    {
                        PurchaseOrderItemId = i.PurchaseOrderItemId,
                        PulseCode = pulseCode,
                        Description = i.Description,
                        AutoIssue = i.AutoIssue,
                        FinalOrder = i.FinalOrder,
                        PackSize = i.PackSizeQuantity,
                        IsNewInventory = i.IsNewInventory,
                        IsPackSizeUpdated = i.IsPacksizeupdated,
                        IsItemEnabledRecently = i.IsItemEnabledRecently,
                        IsItemCodeChangedRecently = i.IsItemCodeChangedRecently,
                        DatabaseInfo = await GetItemInfo(pulseCode),
                        RollingAverage = averages.DefaultIfEmpty(0).Average(),
                        NumAverageWeeks = averages.Count
                    };
                }).Select(t => t.Result)
            });

            async Task<StockOrderRequestItemDatabaseInfo?> GetItemInfo(string pulseCode)
            {
                var dbItem = await _context.InventoryItems.FindAsync(pulseCode);

                if (dbItem is null)
                    return null;

                return new StockOrderRequestItemDatabaseInfo
                {
                    Code = dbItem.Code,
                    PackSize = dbItem.PackSize,
                    Multiplier = dbItem.Multiplier,
                    ManualCount = dbItem.ManualCount
                };
            }
        }

        [HttpPost]
        public async Task SaveOrder(OrderResponse order)
        {
            await _hub.Clients.All.PlaceOrder(order);
        }
    }
}
