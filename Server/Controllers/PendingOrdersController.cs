using DominosStockOrder.Server.Hubs;
using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared;
using DominosStockOrder.Shared.Models.Purchasing;
using DominosStockOrder.Shared.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
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
        private readonly ISavedOrderCacheService _savedOrderCache;
        private readonly ISendGridClient _emailClient;
        private readonly Status _status;

        public PendingOrdersController(IPendingOrdersCacheService pendingOrdersCache, IHubContext<PurchasingHub, IPurchasingClient> hub, IConsolidatedInventoryService consolidatedInventory, ISavedOrderCacheService savedOrderCache, ISendGridClient emailClient, Status status)
        {
            _pendingOrdersCache = pendingOrdersCache;
            _hub = hub;
            _consolidatedInventory = consolidatedInventory;
            _savedOrderCache = savedOrderCache;
            _emailClient = emailClient;
            _status = status;
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
                        Suggested = i.Suggested,
                        AutoIssue = i.AutoIssue,
                        Override = i.Override,
                        PackSize = i.PackSizeQuantity,
                        InTransit = i.InTransit,
                        IsNewInventory = i.IsNewInventory,
                        IsPackSizeUpdated = i.IsPacksizeupdated,
                        IsItemEnabledRecently = i.IsItemEnabledRecently,
                        IsItemCodeChangedRecently = i.IsItemCodeChangedRecently
                    };
                })
            });
        }

        [HttpPost]
        public async Task SaveOrder(StockOrderResponseVM order)
        {
            if (!_savedOrderCache.IsEmpty())
                return;

            _savedOrderCache.SetOrderedItems(order.Items);

            await _hub.Clients.All.PlaceOrder(new OrderResponse
            {
                PurchaseOrderId = order.PurchaseOrderId,
                Items = order.Items.Select(i => new OrderResponseItem
                {
                    PurchaseOrderItemId = i.PurchaseOrderItemId,
                    Override = i.Override
                })
            });
        }

        [HttpGet("Check")]
        public async Task Check()
        {
            // No pending order, no saved order pending response from browser, or order was successfully saved, do nothing.
            if (!_pendingOrdersCache.HasPendingOrder() || _savedOrderCache.IsEmpty() || (_status.IsOrderSuccessful.HasValue && _status.IsOrderSuccessful.Value))
                return;

            // We have a saved order waiting a reply or the order has not been saved or was not successful

            var storeEmail = Environment.GetEnvironmentVariable("STORE_EMAIL");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(storeEmail, "Stock Order"),
                Subject = "Stock Order has not been saved"
            };

            msg.AddContent(MimeType.Text, "There is a pending order that has no been saved to the portal.");
            msg.AddTo(storeEmail);

            var response = await _emailClient.SendEmailAsync(msg);
        }
    }
}
