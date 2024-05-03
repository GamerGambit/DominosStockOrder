using Microsoft.EntityFrameworkCore;

namespace DominosStockOrder.Server.Models
{
    [Keyless]
    public class Settings
    {
        public int NumFoodTheoWeeks { get; set; }
    }
}
