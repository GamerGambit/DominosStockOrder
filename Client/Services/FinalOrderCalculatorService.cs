﻿namespace DominosStockOrder.Client.Services
{
    public class FinalOrderCalculatorService : IFinalOrderCalculatorService
    {
        private readonly IInventoryItemService _inventoryItemService;
        private readonly FoodTheoService _foodTheoService;
        private readonly ILogger<FinalOrderCalculatorService> _logger;

        public FinalOrderCalculatorService(IInventoryItemService inventoryItemService, FoodTheoService foodTheoService, ILogger<FinalOrderCalculatorService> logger)
        {
            _inventoryItemService = inventoryItemService;
            _foodTheoService = foodTheoService;
            _logger = logger;
        }

        public async Task<int> CalculateFinalOrder(string pulseCode)
        {
            var item = await _inventoryItemService.GetInventoryItemFromPulseCodeAsync(pulseCode);

            if (item is null)
            {
                _logger.LogError("Unable to retrieve inventory data for {code}", pulseCode);
                return 0;
            }

            if (!_foodTheoService.Workings.TryGetValue(pulseCode, out var working))
            {
                _logger.LogError("Unable to retrieve workings data for {code}", pulseCode);
                return 0;
            }

            Console.WriteLine($"CalculateFinalOrder {item?.Description}");

            const int originIdeal = 0;
            const int inTransit = 0;

            var currentStock = working.EndingInventory;
            var rollingAvg = working.WeeklyFoodTheo.DefaultIfEmpty(0).Average();
            var soldLastWeekTotal = rollingAvg * item.Multiplier + originIdeal;
            var totalInStoreInTransit = currentStock + inTransit;
            var totalNeeded = totalInStoreInTransit - soldLastWeekTotal;
            var unitsRequired = totalNeeded / item.PackSize; // `unitsRequired` is negative since its the difference between what we have and what we sold

            var sign = MathF.Sign(unitsRequired);

            Console.WriteLine(item.Description);
            Console.WriteLine($"\tSold Last Week: {rollingAvg}");
            Console.WriteLine($"\tToday Opening: {currentStock}");
            Console.WriteLine($"\tSold Last Week Total: {soldLastWeekTotal}");
            Console.WriteLine($"\tTotal In Store/Transit: {totalInStoreInTransit}");
            Console.WriteLine($"\tTotal Stock Needed: {totalNeeded}");
            Console.WriteLine($"\tUnits Required: {unitsRequired}");
            Console.WriteLine($"\tPack Size: {item.PackSize}");
            Console.WriteLine($"\tMultiplier: {item.Multiplier}");

            // If unitsRequired is positive it means we dont need anything as we have more in store than what we used.
            if (sign >= 0)
            {
                return 0;
            }
            else
            {
                return (int)Math.Abs(Math.Round(unitsRequired));
            }
        }
    }
}
