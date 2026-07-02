using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionReasonToDraftCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "DraftCustomers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "DraftCustomers");
        }
    }
}
