using Microsoft.EntityFrameworkCore;

namespace DominosStockOrder.Server.Models
{
    public class StockOrderContext : DbContext
    {
        private readonly IConfiguration _config;

        public StockOrderContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(_config.GetConnectionString("SqliteDB"));
        }

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<ItemInitialFoodTheo> InitialFoodTheos { get; set; }
    }
}
