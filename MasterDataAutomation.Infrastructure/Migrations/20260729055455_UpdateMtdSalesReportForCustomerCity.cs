using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMtdSalesReportForCustomerCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LineName",
                table: "SalesAnalyticsMtdSalesReports",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "LineCode",
                table: "SalesAnalyticsMtdSalesReports",
                newName: "ProductCode");

            migrationBuilder.RenameIndex(
                name: "IX_SalesAnalyticsMtdSalesReports_LineCode",
                table: "SalesAnalyticsMtdSalesReports",
                newName: "IX_SalesAnalyticsMtdSalesReports_ProductCode");

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                table: "SalesAnalyticsMtdSalesReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "SalesAnalyticsMtdSalesReports",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "SalesAnalyticsMtdSalesReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "SalesAnalyticsMtdSalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "SalesAnalyticsMtdSalesReports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "SalesAnalyticsMtdSalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "SalesAnalyticsMtdSalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAfterTax",
                table: "SalesAnalyticsMtdSalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBeforeTax",
                table: "SalesAnalyticsMtdSalesReports",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "SalesAnalyticsMtdSalesReports",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdSalesReports_CityCode",
                table: "SalesAnalyticsMtdSalesReports",
                column: "CityCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdSalesReports_CustomerCode",
                table: "SalesAnalyticsMtdSalesReports",
                column: "CustomerCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SalesAnalyticsMtdSalesReports_CityCode",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropIndex(
                name: "IX_SalesAnalyticsMtdSalesReports_CustomerCode",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "CityCode",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "TotalAfterTax",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "TotalBeforeTax",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "SalesAnalyticsMtdSalesReports");

            migrationBuilder.RenameColumn(
                name: "ProductCode",
                table: "SalesAnalyticsMtdSalesReports",
                newName: "LineCode");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "SalesAnalyticsMtdSalesReports",
                newName: "LineName");

            migrationBuilder.RenameIndex(
                name: "IX_SalesAnalyticsMtdSalesReports_ProductCode",
                table: "SalesAnalyticsMtdSalesReports",
                newName: "IX_SalesAnalyticsMtdSalesReports_LineCode");
        }
    }
}
