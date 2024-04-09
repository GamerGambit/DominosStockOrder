using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DominosStockOrder.Server.Migrations
{
    /// <inheritdoc />
    public partial class MergedInitialWeeklyFoodTheoIntoInventoryItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "InitialFoodTheo",
                table: "InventoryItems",
                type: "REAL",
                nullable: true);

            migrationBuilder.Sql("UPDATE InventoryItems SET InitialFoodTheo = (SELECT InitialFoodTheo FROM InitialFoodTheos WHERE InventoryItems.Code = InitialFoodTheos.PulseCode)");

            migrationBuilder.DropTable(
                name: "InitialFoodTheos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.Sql("INSERT INTO InitialFoodTheos(Code, InitialFoodTheo) SELECT Code, InitialFoodTheo FROM InventoryItems");

            migrationBuilder.DropColumn(
                name: "InitialFoodTheo",
                table: "InventoryItems");
        }
    }
}
