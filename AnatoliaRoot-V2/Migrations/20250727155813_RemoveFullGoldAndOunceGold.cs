using Microsoft.EntityFrameworkCore.Migrations;

namespace AnatoliaRoot_V2.Migrations
{
    public partial class RemoveFullGoldAndOunceGold : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullGold",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "OunceGold",
                table: "GoldPrices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FullGold",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OunceGold",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
