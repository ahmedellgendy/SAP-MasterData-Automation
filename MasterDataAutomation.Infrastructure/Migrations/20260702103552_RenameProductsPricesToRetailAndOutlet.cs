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
            migrationBuilder.Sql(@"
IF COL_LENGTH('Products', 'MarketSellingPrice') IS NOT NULL
   AND COL_LENGTH('Products', 'OutletSellingPrice') IS NULL
BEGIN
    EXEC sp_rename 'Products.MarketSellingPrice', 'OutletSellingPrice', 'COLUMN';
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('ProductRequests', 'MarketSellingPrice') IS NOT NULL
   AND COL_LENGTH('ProductRequests', 'OutletSellingPrice') IS NULL
BEGIN
    EXEC sp_rename 'ProductRequests.MarketSellingPrice', 'OutletSellingPrice', 'COLUMN';
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Products', 'OutletSellingPrice') IS NOT NULL
   AND COL_LENGTH('Products', 'MarketSellingPrice') IS NULL
BEGIN
    EXEC sp_rename 'Products.OutletSellingPrice', 'MarketSellingPrice', 'COLUMN';
END
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('ProductRequests', 'OutletSellingPrice') IS NOT NULL
   AND COL_LENGTH('ProductRequests', 'MarketSellingPrice') IS NULL
BEGIN
    EXEC sp_rename 'ProductRequests.OutletSellingPrice', 'MarketSellingPrice', 'COLUMN';
END
");
        }
    }
}