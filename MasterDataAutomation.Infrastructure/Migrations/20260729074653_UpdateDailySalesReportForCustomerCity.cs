using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDailySalesReportForCustomerCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesAnalyticsDailySalesReports_ProductName",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropIndex(
                name: "IX_SalesAnalyticsDailySalesReports_UploadBatchId",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "SalesAnalyticsDailySalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<string>(
                name: "LineName",
                table: "SalesAnalyticsDailySalesReports",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ImportedAt",
                table: "SalesAnalyticsDailySalesReports",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                table: "SalesAnalyticsDailySalesReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "SalesAnalyticsDailySalesReports",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "SalesAnalyticsDailySalesReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "SalesAnalyticsDailySalesReports",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "SalesAnalyticsDailySalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "SalesAnalyticsDailySalesReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "SalesAnalyticsDailySalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "SalesAnalyticsDailySalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAfterTax",
                table: "SalesAnalyticsDailySalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBeforeTax",
                table: "SalesAnalyticsDailySalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "SalesAnalyticsDailySalesReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_CityCode",
                table: "SalesAnalyticsDailySalesReports",
                column: "CityCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_CustomerCode",
                table: "SalesAnalyticsDailySalesReports",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_ProductCode",
                table: "SalesAnalyticsDailySalesReports",
                column: "ProductCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesAnalyticsDailySalesReports_CityCode",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropIndex(
                name: "IX_SalesAnalyticsDailySalesReports_CustomerCode",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropIndex(
                name: "IX_SalesAnalyticsDailySalesReports_ProductCode",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "CityCode",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "TotalAfterTax",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "TotalBeforeTax",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "SalesAnalyticsDailySalesReports");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "SalesAnalyticsDailySalesReports",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "LineName",
                table: "SalesAnalyticsDailySalesReports",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ImportedAt",
                table: "SalesAnalyticsDailySalesReports",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_ProductName",
                table: "SalesAnalyticsDailySalesReports",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_UploadBatchId",
                table: "SalesAnalyticsDailySalesReports",
                column: "UploadBatchId");
        }
    }
}
