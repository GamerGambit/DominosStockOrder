namespace DominosStockOrder.Client.Services
{
    public class FinalOrderCalculatorService : IFinalOrderCalculatorService
    {
        private readonly IInventoryItemService _inventoryItemService;
        private readonly FoodTheoService _foodTheoService;
        private readonly ILogger<FinalOrderCalculatorService> _logger;
        private readonly IExtraInventoryService _extraInventoryService;

        public FinalOrderCalculatorService(IInventoryItemService inventoryItemService, FoodTheoService foodTheoService, ILogger<FinalOrderCalculatorService> logger, IExtraInventoryService extraInventoryService)
        {
            _inventoryItemService = inventoryItemService;
            _foodTheoService = foodTheoService;
            _logger = logger;
            _extraInventoryService = extraInventoryService;
        }

        public async Task<int> CalculateFinalOrder(string pulseCode, int inTransit)
        {
            var item = await _inventoryItemService.GetInventoryItemFromPulseCodeAsync(pulseCode);

            if (item is null)
            {
                _logger.LogError("Unable to retrieve inventory data for {code}", pulseCode);
                return 0;
            }

            var working = _foodTheoService.GetWorkingsFromPulseCode(pulseCode);
            if (working is null)
            {
                _logger.LogError("Unable to retrieve workings data for {code}", pulseCode);
                return 0;
            }

            Console.WriteLine($"CalculateFinalOrder {item?.Description}");

            float extraIdeal = _extraInventoryService.GetExtraInventoryForPulseCode(pulseCode);
            float inTransitMult = inTransit * item.PackSize;

            var currentStock = working.EndingInventory;
            var rollingAvg = working.WeeklyFoodTheo.DefaultIfEmpty(0).Average();
            var soldLastWeekTotal = rollingAvg * item.Multiplier + extraIdeal;
            var totalInStoreInTransit = currentStock + inTransitMult;
            var totalNeeded = totalInStoreInTransit - soldLastWeekTotal;
            var unitsRequired = totalNeeded / item.PackSize; // `unitsRequired` is negative since its the difference between what we have and what we sold

            var sign = MathF.Sign(unitsRequired);

            Console.WriteLine(item.Description);
            Console.WriteLine($"\tSold Last Week: {rollingAvg}");
            Console.WriteLine($"\tToday Opening: {currentStock}");
            Console.WriteLine($"\tExtra Ideal: {extraIdeal}");
            Console.WriteLine($"\tSold Last Week Total: {soldLastWeekTotal}");
            Console.WriteLine($"\tTotal In Store: {currentStock}");
            Console.WriteLine($"\tTotal In Transit: {inTransitMult}");
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
