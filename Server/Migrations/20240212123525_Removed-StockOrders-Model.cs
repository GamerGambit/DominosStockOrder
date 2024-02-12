using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosStockOrder.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemovedStockOrdersModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_StockOrders_StockOrderPortalOrderId",
                table: "InventoryItems");

            migrationBuilder.DropTable(
                name: "StockOrders");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_StockOrderPortalOrderId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "StockOrderPortalOrderId",
                table: "InventoryItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StockOrderPortalOrderId",
                table: "InventoryItems",
                type: "TEXT",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_StockOrderPortalOrderId",
                table: "InventoryItems",
                column: "StockOrderPortalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_StockOrders_StockOrderPortalOrderId",
                table: "InventoryItems",
                column: "StockOrderPortalOrderId",
                principalTable: "StockOrders",
                principalColumn: "PortalOrderId");
        }
    }
}
