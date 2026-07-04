using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductsPricesToRetailAndOutlet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
        name: "MarketSellingPrice",
        table: "Products",
        newName: "OutletSellingPrice");

            migrationBuilder.RenameColumn(
                name: "MarketSellingPrice",
                table: "ProductRequests",
                newName: "OutletSellingPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
        name: "OutletSellingPrice",
        table: "Products",
        newName: "MarketSellingPrice");

            migrationBuilder.RenameColumn(
                name: "OutletSellingPrice",
                table: "ProductRequests",
                newName: "MarketSellingPrice");
        }
    }
}
