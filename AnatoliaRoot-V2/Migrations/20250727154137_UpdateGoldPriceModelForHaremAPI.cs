using Microsoft.EntityFrameworkCore.Migrations;

namespace AnatoliaRoot_V2.Migrations
{
    public partial class UpdateGoldPriceModelForHaremAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ask",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "Bid",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "Change",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "ChangePercent",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "HighPrice",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "LowPrice",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "OpenPrice",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PrevClosePrice",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PriceGram10k",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PriceGram14k",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PriceGram16k",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PriceGram18k",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PriceGram20k",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PriceGram21k",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PriceGram22k",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PriceGram24k",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "GoldPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "ChangeRate",
                table: "GoldPrices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DayHigh",
                table: "GoldPrices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DayLow",
                table: "GoldPrices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrevClose",
                table: "GoldPrices",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeRate",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "DayHigh",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "DayLow",
                table: "GoldPrices");

            migrationBuilder.DropColumn(
                name: "PrevClose",
                table: "GoldPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "Ask",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Bid",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Change",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ChangePercent",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HighPrice",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LowPrice",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpenPrice",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrevClosePrice",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceGram10k",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceGram14k",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceGram16k",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceGram18k",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceGram20k",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceGram21k",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceGram22k",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceGram24k",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "GoldPrices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
