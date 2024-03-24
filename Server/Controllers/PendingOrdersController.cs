using DominosStockOrder.Server.Hubs;
using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared.Models.Purchasing;
using DominosStockOrder.Shared.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

using System.Text.RegularExpressions;

namespace DominosStockOrder.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PendingOrdersController : ControllerBase
    {
        private readonly IPendingOrdersCacheService _pendingOrdersCache;
        private readonly IHubContext<PurchasingHub, IPurchasingClient> _hub;
        private readonly IConsolidatedInventoryService _consolidatedInventory;

        public PendingOrdersController(IPendingOrdersCacheService pendingOrdersCache, IHubContext<PurchasingHub, IPurchasingClient> hub, IConsolidatedInventoryService consolidatedInventory)
        {
            _pendingOrdersCache = pendingOrdersCache;
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
                Items = o.Items.Select(i =>
                {
                    var pulseCode = Regex.Replace(i.Code, @"\D+$", string.Empty);
                    var averages = _consolidatedInventory.GetItemFoodTheos(pulseCode);
                    return new StockOrderRequestItemVM
                    {
                        PurchaseOrderItemId = i.PurchaseOrderItemId,
                        PulseCode = pulseCode,
                        Description = i.Description,
                        AutoIssue = i.AutoIssue,
                        Override = i.Override,
                        PackSize = i.PackSizeQuantity,
                        IsNewInventory = i.IsNewInventory,
                        IsPackSizeUpdated = i.IsPacksizeupdated,
                        IsItemEnabledRecently = i.IsItemEnabledRecently,
                        IsItemCodeChangedRecently = i.IsItemCodeChangedRecently
                    };
                })
            });
        }

        [HttpPost]
        public async Task SaveOrder(OrderResponse order)
        {
            await _hub.Clients.All.PlaceOrder(order);
        }
    }
}
