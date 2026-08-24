using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesAnalyticsDailyVisitReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesAnalyticsDailyVisitReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupervisorName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CityName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    VisitCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SalesRepCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesRepName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    VisitStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NegativeReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SuccessfulVisitValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VisitStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VisitEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VisitDurationText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UploadBatchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAnalyticsDailyVisitReports", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailyVisitReports_CustomerCode",
                table: "SalesAnalyticsDailyVisitReports",
                column: "CustomerCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailyVisitReports_ReportDate",
                table: "SalesAnalyticsDailyVisitReports",
                column: "ReportDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailyVisitReports_SalesRepCode",
                table: "SalesAnalyticsDailyVisitReports",
                column: "SalesRepCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailyVisitReports_UploadBatchId",
                table: "SalesAnalyticsDailyVisitReports",
                column: "UploadBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsDailyVisitReports_VisitStatus",
                table: "SalesAnalyticsDailyVisitReports",
                column: "VisitStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesAnalyticsDailyVisitReports");
        }
    }
}
