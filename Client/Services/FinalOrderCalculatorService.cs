using DominosStockOrder.Shared;
using System.ComponentModel.Design;

namespace DominosStockOrder.Client.Services
{
    public class FinalOrderCalculatorService : IFinalOrderCalculatorService
    {
        private readonly IInventoryItemService _inventoryItemService;
        private readonly FoodTheoService _foodTheoService;
        private readonly ILogger<FinalOrderCalculatorService> _logger;
        private readonly IExtraInventoryService _extraInventoryService;
        private readonly ITransferService _transferService;

        public FinalOrderCalculatorService(IInventoryItemService inventoryItemService, FoodTheoService foodTheoService, ILogger<FinalOrderCalculatorService> logger, IExtraInventoryService extraInventoryService, ITransferService transferService)
        {
            _inventoryItemService = inventoryItemService;
            _foodTheoService = foodTheoService;
            _logger = logger;
            _extraInventoryService = extraInventoryService;
            _transferService = transferService;
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

            var transfer = _transferService.GetTransferForPulseCode(pulseCode);
            float transferred = 0;

            if (transfer is not null)
            {
                // DO NOT multiply by packsize as this is in pulse units, same as ending inventory
                transferred = transfer.Quantity;

                // `transferred` is subtracted from `currentStock`.
                // for items that are borrowed we want these subtracted so we effectively have less in-store so more is ordered.
                // for items that are lent out we want these added on so we dont order extra as its expected to be returned at some point.
                // we invert `transferred` when the item is borrowed so we subtract a negative (add)
                if (transfer.TransferType == Models.TransferType.Lend)
                {
                    transferred *= -1;
                }
            }

            var numInitialTheoWeeks = Constants.NumFoodTheoWeeks - working.WeeklyFoodTheo.Count;
            var initialWeeklyTheo = (item.InitialFoodTheo ?? 0);
            var defaultWeeklyTheo = initialWeeklyTheo * Constants.NumFoodTheoWeeks;
            var roundedTransfer = Math.Ceiling(transferred / item.PackSize) * item.PackSize;
            var currentStock = working.EndingInventory - roundedTransfer;

            var weeklyTheos = working.WeeklyFoodTheo.Select(x => x.IdealUsage).ToList();

            if (numInitialTheoWeeks > 0)
            {
                var range = Enumerable.Range(0, numInitialTheoWeeks).Select(x => initialWeeklyTheo);
                weeklyTheos.AddRange(range);
            }

            var rollingAvg = weeklyTheos.Average();
            var soldLastWeekTotal = rollingAvg * item.Multiplier + extraIdeal;
            var totalInStoreInTransit = currentStock + inTransitMult;
            var totalNeeded = totalInStoreInTransit - soldLastWeekTotal;
            var unitsRequired = totalNeeded / item.PackSize; // `unitsRequired` is negative since its the difference between what we have and what we sold

            Console.WriteLine(item.Description);
            Console.WriteLine($"\tInitial Food Theo: {initialWeeklyTheo} (used for {numInitialTheoWeeks} weeks)");
            Console.WriteLine($"\tSold Last Week: {rollingAvg}");
            Console.WriteLine($"\tToday Ending: {working.EndingInventory}");
            Console.WriteLine($"\tExtra Ideal: {extraIdeal}");
            Console.WriteLine($"\tSold Last Week Total: {soldLastWeekTotal}");
            Console.WriteLine($"\tTransfered: {transferred} {transfer?.TransferType}");
            Console.WriteLine($"\t\tOrdering an extra: {roundedTransfer}");
            Console.WriteLine($"\tTotal In Store: {currentStock}");
            Console.WriteLine($"\tTotal In Transit: {inTransitMult}");
            Console.WriteLine($"\tTotal Stock Needed: {totalNeeded}");
            Console.WriteLine($"\tUnits Required: {unitsRequired}");
            Console.WriteLine($"\tPack Size: {item.PackSize}");
            Console.WriteLine($"\tMultiplier: {item.Multiplier}");
            Console.Write($"\tFinal Order: ");

            if (unitsRequired >= 0)
            {
                Console.WriteLine(0);
                return 0;
            }

            var abs = Math.Abs(unitsRequired);

            if (abs > 0 && abs < 1)
            {
                Console.WriteLine(1);
                return 1;
            }

            var final = (int)Math.Round(abs);
            Console.WriteLine(final);
            return final;
        }
    }
}
