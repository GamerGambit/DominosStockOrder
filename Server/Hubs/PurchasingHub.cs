using DominosStockOrder.Server.Models;
using DominosStockOrder.Server.Services;
using DominosStockOrder.Shared;
using DominosStockOrder.Shared.Models.Purchasing;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.SignalR;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;

namespace DominosStockOrder.Server.Hubs
{
    public class PurchasingHub : Hub<IPurchasingClient>
    {
        private int _numClients = 0;
        private readonly Status _statusService;
        private readonly ILogger<PurchasingHub> _logger;
        private readonly ISavedOrderCacheService _savedOrderCache;
        private readonly ISendGridClient _emailClient;
        private readonly StockOrderContext _context;
        private readonly IConsolidatedInventoryService _consolidatedInventory;

        public PurchasingHub(Status statusService, ILogger<PurchasingHub> logger, ISavedOrderCacheService savedOrderCacheService, ISendGridClient emailClient, StockOrderContext context, IConsolidatedInventoryService consolidatedInventory)
        {
            _statusService = statusService;
            _logger = logger;
            _savedOrderCache = savedOrderCacheService;
            _emailClient = emailClient;
            _context = context;
            _consolidatedInventory = consolidatedInventory;
        }

        public async Task SetPendingOrders(IEnumerable<OrderRequest> orders, IInventoryUpdaterService inventoryUpdater, IPendingOrdersCacheService pendingOrdersCache)
        {
            pendingOrdersCache.ClearOrders();

            if (!orders.Any())
                return;

            // ignore other orders
            var order = orders.First();

            pendingOrdersCache.AddOrder(order);

            _statusService.EODRun |= order.OrderDate.Date == DateTime.Now.Date;
            _logger.LogInformation("Received order for {date}, setting EODRun to {eod}", order.DeliveryDate.ToString("d"), _statusService.EODRun);

            foreach (var item in order.Items)
            {
                await inventoryUpdater.AddOrUpdateInventoryItem(item.PurchaseOrderItemId, item.Code, item.Description, item.PackSizeQuantity);
            }
        }

        public async Task PlaceOrder(OrderResponse order)
        {
            await Clients.All.PlaceOrder(order);
        }

        public async Task OrderSuccessful()
        {
            _logger.LogInformation("Order placed successfully");

            _statusService.IsOrderSuccessful = true;
            _statusService.OrderError = string.Empty;

            await SendOrderSuccessEmail();

            _savedOrderCache.Clear();
        }

        public async Task OrderFailed(string error)
        {
            _logger.LogError("Order was unsuccessully: {error}", error);

            _statusService.IsOrderSuccessful = false;
            _statusService.OrderError = error;
            _savedOrderCache.Clear();
        }

        public override Task OnConnectedAsync()
        {
            ++_numClients;
            _statusService.IsConnectedToBrowser = true;

            _logger.LogInformation("New browser connection");

            // Try to send any pending orders.
            if (_savedOrderCache.HasSavedOrder())
            {
                _logger.LogInformation("Saved order cached, attempting to send");

                var order = _savedOrderCache.GetSavedOrder();
                Clients.Caller.PlaceOrder(new OrderResponse
                {
                    PurchaseOrderId = order.PurchaseOrderId,
                    Items = order.Items.Select(i => new OrderResponseItem
                    {
                        PurchaseOrderItemId = i.PurchaseOrderItemId,
                        Override = i.Override
                    })
                });
            }

            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            --_numClients;
            _statusService.IsConnectedToBrowser = _numClients > 0;
            return Task.CompletedTask;
        }

        private async Task SendOrderSuccessEmail()
        {
            var storeEmail = Environment.GetEnvironmentVariable("STORE_EMAIL");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(storeEmail, "Stock Order"),
                Subject = "Stock Order Saved"
            };

            var builder = new StringBuilder();
            builder.Append("<table border='1'><thead><tr>");
            builder.Append("<th>Code</th>");
            builder.Append("<th>Description</th>");
            builder.Append("<th>Override</th>");
            builder.Append("<th>8-week Avg</th>");
            builder.Append("<th>In Store</th>");
            builder.Append("<th>In Transit</th>");
            builder.Append("<th>Extra Theo</th>");
            builder.Append("<th>Transferred</th>");
            builder.Append("<th>Initial Weekly Theo</th>");
            builder.Append("<th>Pack Size</th>");
            builder.Append("<th>Multiplier</th>");
            builder.Append("</tr></thead><tbody>");

            foreach (var item in _savedOrderCache.GetSavedOrder().Items.OrderBy(i => i.PulseCode))
            {
                var invItem = await _context.InventoryItems.FindAsync(item.PulseCode);
                var inStore = _consolidatedInventory.GetItemEndingInventory(item.PulseCode);
                var weeklyTheos = _consolidatedInventory.GetItemFoodTheos(item.PulseCode).Select(x => x.IdealUsage).ToList();

                var initialWeeklyTheo = invItem.InitialFoodTheo ?? 0;
                var neededInitialTheos = Constants.NumFoodTheoWeeks - weeklyTheos.Count;

                for(var i = 0; i < neededInitialTheos; ++i)
                {
                    weeklyTheos.Add(initialWeeklyTheo);
                }

                var rollingAvg = weeklyTheos.Average();

                builder.Append("<tr>");
                builder.Append($"<td>{item.PulseCode}</td>");
                builder.Append($"<td>{invItem.Description}</td>");
                builder.Append($"<td>{item.Override}</td>");
                builder.Append($"<td>{rollingAvg}</td>");
                builder.Append($"<td>{inStore}</td>");
                builder.Append($"<td>{item.InTransit}</td>");
                builder.Append($"<td>{item.ExtraIdeal}</td>");
                builder.Append($"<td>{item.Transferred}</td>");
                builder.Append($"<td>{initialWeeklyTheo} (Used for {neededInitialTheos} weeks)</td>");
                builder.Append($"<td>{invItem.PackSize}</td>");
                builder.Append($"<td>{invItem.Multiplier}</td>");
                builder.Append("</tr>");
            }

            builder.Append("</tbody></table>");

            msg.AddContent(MimeType.Html, builder.ToString());
            msg.AddTo(storeEmail);

            var response = await _emailClient.SendEmailAsync(msg);
        }
    }
}
