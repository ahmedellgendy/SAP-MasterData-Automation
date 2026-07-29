using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesAnalyticsMtdSalesReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesAnalyticsMtdSalesReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LineCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SalesAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadBatchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAnalyticsMtdSalesReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdSalesReports_LineCode",
                table: "SalesAnalyticsMtdSalesReports",
                column: "LineCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdSalesReports_Year_Month_ToDate",
                table: "SalesAnalyticsMtdSalesReports",
                columns: new[] { "Year", "Month", "ToDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesAnalyticsMtdSalesReports");
        }
    }
}
