using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesAnalyticsMtdVisitReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesAnalyticsMtdVisitReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SupervisorName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    VisitCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SalesRepCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesRepName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    VisitStatus = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NegativeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SuccessfulVisitValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VisitStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VisitEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VisitDurationText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadBatchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAnalyticsMtdVisitReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdVisitReports_CustomerCode",
                table: "SalesAnalyticsMtdVisitReports",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdVisitReports_SalesRepCode",
                table: "SalesAnalyticsMtdVisitReports",
                column: "SalesRepCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdVisitReports_VisitCode",
                table: "SalesAnalyticsMtdVisitReports",
                column: "VisitCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdVisitReports_VisitDate",
                table: "SalesAnalyticsMtdVisitReports",
                column: "VisitDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsMtdVisitReports_Year_Month_ToDate",
                table: "SalesAnalyticsMtdVisitReports",
                columns: new[] { "Year", "Month", "ToDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesAnalyticsMtdVisitReports");
        }
    }
}
