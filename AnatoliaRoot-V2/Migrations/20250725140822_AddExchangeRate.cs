using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnatoliaRoot_V2.Migrations
{
    public partial class AddExchangeRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    UsdRate = table.Column<decimal>(nullable: false),
                    EurRate = table.Column<decimal>(nullable: false),
                    UsdTry = table.Column<decimal>(nullable: false),
                    EurTry = table.Column<decimal>(nullable: false),
                    Source = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRates");
        }
    }
}
