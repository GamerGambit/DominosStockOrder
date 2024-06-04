using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominosStockOrder.Server.Models
{
    public class Settings
    {
        /// <summary>
        /// This is just a dummy property to allow us to use Entity Framework Core
        /// to write changes to this record. The database should only contain
        /// a single record for settings.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int NumFoodTheoWeeks { get; set; }
    }
}
