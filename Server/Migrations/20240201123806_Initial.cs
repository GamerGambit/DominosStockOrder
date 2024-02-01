using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosStockOrder.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockOrders",
                columns: table => new
                {
                    PortalOrderId = table.Column<string>(type: "TEXT", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOrders", x => x.PortalOrderId);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    PortalItemId = table.Column<string>(type: "TEXT", nullable: true),
                    PackSize = table.Column<float>(type: "REAL", nullable: false),
                    Multiplier = table.Column<float>(type: "REAL", nullable: false),
                    ManualCount = table.Column<bool>(type: "INTEGER", nullable: false),
                    StockOrderPortalOrderId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Code);
                    table.ForeignKey(
                        name: "FK_InventoryItems_StockOrders_StockOrderPortalOrderId",
                        column: x => x.StockOrderPortalOrderId,
                        principalTable: "StockOrders",
                        principalColumn: "PortalOrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_StockOrderPortalOrderId",
                table: "InventoryItems",
                column: "StockOrderPortalOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "StockOrders");
        }
    }
}
