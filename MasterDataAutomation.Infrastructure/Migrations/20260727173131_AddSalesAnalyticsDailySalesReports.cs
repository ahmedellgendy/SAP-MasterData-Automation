using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesAnalyticsDailySalesReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesAnalyticsDailySalesReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LineCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    SalesAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UploadBatchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAnalyticsDailySalesReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_LineCode",
                table: "SalesAnalyticsDailySalesReports",
                column: "LineCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_ProductName",
                table: "SalesAnalyticsDailySalesReports",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_ReportDate",
                table: "SalesAnalyticsDailySalesReports",
                column: "ReportDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailySalesReports_UploadBatchId",
                table: "SalesAnalyticsDailySalesReports",
                column: "UploadBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesAnalyticsDailySalesReports");
        }
    }
}
