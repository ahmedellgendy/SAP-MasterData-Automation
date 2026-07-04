using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PiecesCount = table.Column<int>(type: "int", nullable: false),
                    PiecePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HasBonus = table.Column<bool>(type: "bit", nullable: false),
                    BonusQuantity = table.Column<int>(type: "int", nullable: true),
                    OutletSellingPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RetailSellingPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountsManagerReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutiveManagerReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAsProductAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequests_ItemCode",
                table: "ProductRequests",
                column: "ItemCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductRequests");
        }
    }
}
