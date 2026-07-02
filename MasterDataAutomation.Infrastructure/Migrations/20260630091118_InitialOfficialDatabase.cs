using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialOfficialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DraftCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Line = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Market = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Branch = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalesDistrict = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Draft")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenerationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CustomersCount = table.Column<int>(type: "int", nullable: false),
                    FirstBpCode = table.Column<int>(type: "int", nullable: false),
                    LastBpCode = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerationHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DraftCustomers");

            migrationBuilder.DropTable(
                name: "GenerationHistories");

            migrationBuilder.DropTable(
                name: "SystemSettings");
        }
    }
}
