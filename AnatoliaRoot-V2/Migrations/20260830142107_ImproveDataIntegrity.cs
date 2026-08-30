using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnatoliaRoot_V2.Migrations
{
    /// <inheritdoc />
    public partial class ImproveDataIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuarterGold",
                table: "GoldPrices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrevClose",
                table: "GoldPrices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "HalfGold",
                table: "GoldPrices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "GramGold",
                table: "GoldPrices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DayLow",
                table: "GoldPrices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DayHigh",
                table: "GoldPrices",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ChangeRate",
                table: "GoldPrices",
                type: "decimal(9,4)",
                precision: 9,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UsdTry",
                table: "ExchangeRates",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "UsdRate",
                table: "ExchangeRates",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EurTry",
                table: "ExchangeRates",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EurRate",
                table: "ExchangeRates",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<long>(
                name: "SourceTimestamp",
                table: "ExchangeRates",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoldPrices_Date",
                table: "GoldPrices",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_GoldPrices_Timestamp",
                table: "GoldPrices",
                column: "Timestamp",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_GoldPrices_ValidValues",
                table: "GoldPrices",
                sql: "[GramGold] > 0 AND [QuarterGold] > 0 AND [HalfGold] > 0 AND [DayHigh] > 0 AND [DayLow] > 0 AND [DayLow] <= [DayHigh] AND [PrevClose] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_Date",
                table: "ExchangeRates",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_SourceTimestamp",
                table: "ExchangeRates",
                column: "SourceTimestamp",
                unique: true,
                filter: "[SourceTimestamp] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ExchangeRates_PositiveValues",
                table: "ExchangeRates",
                sql: "[UsdRate] > 0 AND [EurRate] > 0 AND [UsdTry] > 0 AND [EurTry] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId_Name",
                table: "Categories",
                columns: new[] { "ParentCategoryId", "Name" },
                unique: true,
                filter: "[ParentCategoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Root_Name",
                table: "Categories",
                column: "Name",
                unique: true,
                filter: "[ParentCategoryId] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Categories_NotSelfParent",
                table: "Categories",
                sql: "[ParentCategoryId] IS NULL OR [ParentCategoryId] <> [Id]");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_GoldPrices_Date",
                table: "GoldPrices");

            migrationBuilder.DropIndex(
                name: "IX_GoldPrices_Timestamp",
                table: "GoldPrices");

            migrationBuilder.DropCheckConstraint(
                name: "CK_GoldPrices_ValidValues",
                table: "GoldPrices");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_Date",
                table: "ExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_SourceTimestamp",
                table: "ExchangeRates");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ExchangeRates_PositiveValues",
                table: "ExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Root_Name",
                table: "Categories");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Categories_NotSelfParent",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SourceTimestamp",
                table: "ExchangeRates");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuarterGold",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrevClose",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "HalfGold",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "GramGold",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "DayLow",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "DayHigh",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "ChangeRate",
                table: "GoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,4)",
                oldPrecision: 9,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "UsdTry",
                table: "ExchangeRates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "UsdRate",
                table: "ExchangeRates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)",
                oldPrecision: 18,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "EurTry",
                table: "ExchangeRates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "EurRate",
                table: "ExchangeRates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,8)",
                oldPrecision: 18,
                oldScale: 8);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
