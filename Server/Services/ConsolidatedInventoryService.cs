
using DominosStockOrder.Server.Models;
using DominosStockOrder.Server.PulseApi;
using DominosStockOrder.Shared;

using System.Text.RegularExpressions;

namespace DominosStockOrder.Server.Services
{
    public class ConsolidatedInventoryService : IConsolidatedInventoryService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPulseApiClient _pulse;
        private readonly ILogger<ConsolidatedInventoryService> _logger;
        private readonly Dictionary<string, List<float>> _itemFoodTheos = [];

        private static readonly List<float> zeroList = new(0);

        public ConsolidatedInventoryService(IPulseApiClient pulse, ILogger<ConsolidatedInventoryService> logger, IServiceProvider serviceProvider)
        {
            _pulse = pulse;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task FetchConsolidatedInventoryAsync()
        {
            for (int i = 0; i < Constants.NumFoodTheoWeeks; i++)
            {
                await FetchConsolidatedInventoryWithWeekOffset(i);
            }
        }

        public IList<float> GetItemFoodTheos(string pulseCode)
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StockOrderContext>();
            var initialFoodTheoEntry = context.InitialFoodTheos.Find(pulseCode);

            if (_itemFoodTheos.TryGetValue(pulseCode, out var foodTheos))
            {
                var missingWeeks = Constants.NumFoodTheoWeeks - foodTheos.Count;
                if (missingWeeks > 0)
                {
                    // If we are missing weeks of theo, add the projected usage by the number of missing weeks
                    // to the original list.
                    var initialFoodTheo = initialFoodTheoEntry?.InitialFoodTheo ?? 0;
                    foodTheos.Add(initialFoodTheo * missingWeeks);
                }

                return foodTheos;
            }

            return zeroList;
        }

        /// <summary>
        /// Calculate the week to run food theo for. If its before Sunday, it will run the previous week's Mon-Sun.
        /// </summary>
        /// <param name="weekOffset">How many previous weeks to get food theo for.</param>
        /// <returns>A single week of food theo</returns>
        private async Task FetchConsolidatedInventoryWithWeekOffset(int weekOffset)
        {
            var today = DateTime.Now;
            var sunday = today.AddDays(-(int)today.DayOfWeek).AddDays(-(weekOffset * 7));
            var monday = sunday.AddDays(-6);

            var itemUsages = await _pulse.ConsolidatedInventoryAsync(monday, sunday);
            ProcessConsolidatedInventory(itemUsages);
        }

        private void ProcessConsolidatedInventory(ICollection<DailyInventory> inventory)
        {
            foreach (var item in inventory)
            {
                ProcessSingleItemInventory(item);
            }
        }

        private void ProcessSingleItemInventory(DailyInventory inventory)
        {
            var match = Regex.Match(inventory.Description, @"^\(([\d\w]+)\) (.*)$");
            var code = match.Groups[1].Value;

            if (!_itemFoodTheos.ContainsKey(code))
            {
                _itemFoodTheos.Add(code, new  List<float>());
            }

            _itemFoodTheos[code].Add(inventory.IdealUsage);
        }
    }
}
