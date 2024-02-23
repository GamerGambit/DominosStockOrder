using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosStockOrder.Server.Migrations
{
    /// <inheritdoc />
    public partial class FoodTheoPulledFromPulse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAverages");

            migrationBuilder.CreateTable(
                name: "InitialFoodTheos",
                columns: table => new
                {
                    PulseCode = table.Column<string>(type: "TEXT", nullable: false),
                    InitialFoodTheo = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialFoodTheos", x => x.PulseCode);
                    table.ForeignKey(
                        name: "FK_InitialFoodTheos_InventoryItems_PulseCode",
                        column: x => x.PulseCode,
                        principalTable: "InventoryItems",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InitialFoodTheos");

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
    }
}
