using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosStockOrder.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedItemAveragesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemAverages",
                columns: table => new
                {
                    ItemCode = table.Column<string>(type: "TEXT", nullable: false),
                    WeekEnding = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FoodTheo = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAverages", x => new { x.ItemCode, x.WeekEnding });
                    table.ForeignKey(
                        name: "FK_ItemAverages_InventoryItems_ItemCode",
                        column: x => x.ItemCode,
                        principalTable: "InventoryItems",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAverages");
        }
    }
}
