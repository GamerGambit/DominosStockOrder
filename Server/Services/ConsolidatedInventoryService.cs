using DominosStockOrder.Server.PulseApi;
using DominosStockOrder.Shared;
using DominosStockOrder.Shared.DTOs;
using System.Text.RegularExpressions;

namespace DominosStockOrder.Server.Services
{
    public class ConsolidatedInventoryService : IConsolidatedInventoryService
    {
        private class EndingInventoryData
        {
            public DateTime Date { get; set; }
            public Dictionary<string, float> EndingInventoryDict = [];
        }

        private readonly IPulseApiClient _pulse;
        private readonly ILogger<ConsolidatedInventoryService> _logger;
        private readonly Dictionary<string, List<ItemWeeklyFoodTheo>> _weeklyTheoDict = [];
        private readonly EndingInventoryData _endingInventoryData = new();

        private DateTime _processingWeekEnding;

        public ConsolidatedInventoryService(IPulseApiClient pulse, ILogger<ConsolidatedInventoryService> logger)
        {
            _pulse = pulse;
            _logger = logger;
        }

        public async Task FetchWeeklyFoodTheoAsync()
        {
            _weeklyTheoDict.Clear();

            // Reset before we process any food usage just in case
            _processingWeekEnding = new();

            for (int i = 0; i < Constants.NumFoodTheoWeeks; i++)
            {
                try
                {
                    await FetchConsolidatedInventoryWithWeekOffset(i);
                }
                catch (Exception ex) when (ex is HttpRequestException)
                {
                    _logger.LogError(ex, "Exception when running Pulse Consolidated Inventory");
                    return;
                }
            }

            // Reset after we process food usage just in case
            _processingWeekEnding = new();
        }

        public async Task FetchEndingInventoryAsync(DateTime date)
        {
            _endingInventoryData.Date = date;
            _endingInventoryData.EndingInventoryDict.Clear();

            var itemUsages = await _pulse.ConsolidatedInventoryAsync(date, date);

            foreach (var itemUsage in itemUsages)
            {
                var match = Regex.Match(itemUsage.Description, @"^\(([\d\w]+)\) (.*)$");
                var code = match.Groups[1].Value;

                _endingInventoryData.EndingInventoryDict.Add(code, itemUsage.EndingInventory);
            }
        }

        public IList<ItemWeeklyFoodTheo> GetItemFoodTheos(string pulseCode)
        {
            if (!_weeklyTheoDict.TryGetValue(pulseCode, out var data))
                return [];

            return data;
        }

        public float GetItemEndingInventory(string pulseCode)
        {
            if (!_endingInventoryData.EndingInventoryDict.TryGetValue(pulseCode, out var endingInventory))
                return 0;

            return endingInventory;
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
            _processingWeekEnding = sunday;

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

            if (!_weeklyTheoDict.TryGetValue(code, out var data))
            {
                data = [];
                _weeklyTheoDict.Add(code, data);
            }

            data.Add(new ItemWeeklyFoodTheo
            {
                WeekEnding = _processingWeekEnding,
                IdealUsage = inventory.IdealUsage
            });
        }
    }
}
