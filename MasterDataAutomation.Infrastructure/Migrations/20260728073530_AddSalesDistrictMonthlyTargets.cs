using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesDistrictMonthlyTargets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesDistrictMonthlyTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SalesDistrictCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesDistrictName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MonthlySalesTarget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlannedVisits = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesDistrictMonthlyTargets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDistrictMonthlyTargets_BranchCode",
                table: "SalesDistrictMonthlyTargets",
                column: "BranchCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDistrictMonthlyTargets_SalesDistrictCode",
                table: "SalesDistrictMonthlyTargets",
                column: "SalesDistrictCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDistrictMonthlyTargets_Year_Month_SalesDistrictCode",
                table: "SalesDistrictMonthlyTargets",
                columns: new[] { "Year", "Month", "SalesDistrictCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesDistrictMonthlyTargets");
        }
    }
}
