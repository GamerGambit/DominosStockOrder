using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosStockOrder.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedInventoryItemDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "InventoryItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "InventoryItems");
        }
    }
}
