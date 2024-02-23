namespace DominosStockOrder.Shared.ViewModels
{
    public class WorkingsVM
    {
        public string Description { get; set; }

        /// <summary>
        /// Last <see cref="Constants.NumFoodTheoWeeks"/> weeks of food theo
        /// </summary>
        public List<float> WeeklyFoodTheo { get; set; }
    }
}
