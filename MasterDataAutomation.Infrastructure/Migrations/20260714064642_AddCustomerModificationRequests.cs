using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerModificationRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerModificationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MarketCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MarketName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CurrentCustomerType = table.Column<int>(type: "int", nullable: false),
                    ModificationType = table.Column<int>(type: "int", nullable: false),
                    NewMarketName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerModificationRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerModificationRequests_MarketCode",
                table: "CustomerModificationRequests",
                column: "MarketCode");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerModificationRequests_Status",
                table: "CustomerModificationRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerModificationRequests");
        }
    }
}
