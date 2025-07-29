using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnatoliaRoot_V2.Migrations
{
    public partial class AddGoldPricesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GoldPrices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    GramGold = table.Column<decimal>(nullable: false),
                    QuarterGold = table.Column<decimal>(nullable: false),
                    HalfGold = table.Column<decimal>(nullable: false),
                    FullGold = table.Column<decimal>(nullable: false),
                    OunceGold = table.Column<decimal>(nullable: false),
                    Source = table.Column<string>(maxLength: 100, nullable: true),
                    Currency = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoldPrices", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GoldPrices");
        }
    }
}
