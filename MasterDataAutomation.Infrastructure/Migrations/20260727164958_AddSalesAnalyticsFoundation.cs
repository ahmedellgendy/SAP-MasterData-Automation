using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesAnalyticsFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesAnalyticsCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CustomerAccountGroup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SalesDistrictCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SalesDistrictName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CustomerClassificationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerClassificationName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IncotermsCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IncotermsName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SearchTerm = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SearchTerm2 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SourceCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SourceCreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UploadBatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAnalyticsCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesAnalyticsSalesReps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesRepCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesRepName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RegionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegionName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    InternalCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SearchTerm2 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SourceCreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SourceCreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UploadBatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAnalyticsSalesReps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesAnalyticsUploadBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    ImportedRows = table.Column<int>(type: "int", nullable: false),
                    FailedRows = table.Column<int>(type: "int", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAnalyticsUploadBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesRepRouteAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesRepCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesRepName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SalesDistrictCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesDistrictName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UploadBatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesRepRouteAssignments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsCustomers_BranchCode",
                table: "SalesAnalyticsCustomers",
                column: "BranchCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsCustomers_CustomerCode",
                table: "SalesAnalyticsCustomers",
                column: "CustomerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsCustomers_SalesDistrictCode",
                table: "SalesAnalyticsCustomers",
                column: "SalesDistrictCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsCustomers_UploadBatchId",
                table: "SalesAnalyticsCustomers",
                column: "UploadBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsSalesReps_BranchCode",
                table: "SalesAnalyticsSalesReps",
                column: "BranchCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsSalesReps_InternalCode",
                table: "SalesAnalyticsSalesReps",
                column: "InternalCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsSalesReps_SalesRepCode",
                table: "SalesAnalyticsSalesReps",
                column: "SalesRepCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsSalesReps_UploadBatchId",
                table: "SalesAnalyticsSalesReps",
                column: "UploadBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsUploadBatches_FileType",
                table: "SalesAnalyticsUploadBatches",
                column: "FileType");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsUploadBatches_ReportDate",
                table: "SalesAnalyticsUploadBatches",
                column: "ReportDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAnalyticsUploadBatches_UploadDate",
                table: "SalesAnalyticsUploadBatches",
                column: "UploadDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesRepRouteAssignments_BranchCode",
                table: "SalesRepRouteAssignments",
                column: "BranchCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesRepRouteAssignments_IsActive",
                table: "SalesRepRouteAssignments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SalesRepRouteAssignments_SalesDistrictCode",
                table: "SalesRepRouteAssignments",
                column: "SalesDistrictCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesRepRouteAssignments_SalesRepCode",
                table: "SalesRepRouteAssignments",
                column: "SalesRepCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesRepRouteAssignments_UploadBatchId",
                table: "SalesRepRouteAssignments",
                column: "UploadBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesAnalyticsCustomers");

            migrationBuilder.DropTable(
                name: "SalesAnalyticsSalesReps");

            migrationBuilder.DropTable(
                name: "SalesAnalyticsUploadBatches");

            migrationBuilder.DropTable(
                name: "SalesRepRouteAssignments");
        }
    }
}
