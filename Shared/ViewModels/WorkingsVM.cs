using DominosStockOrder.Shared.DTOs;

namespace DominosStockOrder.Shared.ViewModels
{
    public class WorkingsVM
    {
        public string PulseCode { get; set; }

        /// <summary>
        /// Last <see cref="Constants.NumFoodTheoWeeks"/> weeks of food theo
        /// </summary>
        public List<ItemWeeklyFoodTheo> WeeklyFoodTheo { get; set; }
        public float EndingInventory { get; set; }
    }
}
