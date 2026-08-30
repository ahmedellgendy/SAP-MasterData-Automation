using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCustomerCareSlaPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CustomerCareSlaPolicies",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "EscalationMinutes", "FirstResponseMinutes", "IsActive", "Name", "Priority", "ResolutionMinutes", "SortOrder", "SubCategoryId", "TicketType", "UpdatedAt", "UseBusinessHours" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Default SLA for customer complaints.", 480, 120, true, "Default Complaint SLA", null, 1440, 100, null, 1, null, false },
                    { 2, null, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "SLA for high-priority complaints.", 180, 60, true, "High Priority Complaint SLA", 3, 480, 20, null, 1, null, false },
                    { 3, null, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "SLA for critical customer complaints.", 60, 30, true, "Critical Complaint SLA", 4, 240, 10, null, 1, null, false },
                    { 4, 5, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Default SLA for product quality complaints.", 120, 60, true, "Quality Complaint SLA", null, 480, 15, null, 1, null, false },
                    { 5, 5, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Critical SLA for quality complaints involving serious risk.", 30, 15, true, "Critical Quality Complaint SLA", 4, 180, 1, null, 1, null, false },
                    { 6, null, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Default SLA for customer requests.", 1440, 240, true, "Customer Request SLA", null, 2880, 100, null, 2, null, false },
                    { 7, null, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Default SLA for customer inquiries.", null, 120, true, "Customer Inquiry SLA", null, 480, 100, null, 3, null, false },
                    { 8, null, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Default SLA for transferred customer cases.", 120, 60, true, "Transfer SLA", null, 240, 100, null, 4, null, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CustomerCareSlaPolicies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CustomerCareSlaPolicies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CustomerCareSlaPolicies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CustomerCareSlaPolicies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CustomerCareSlaPolicies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CustomerCareSlaPolicies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CustomerCareSlaPolicies",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CustomerCareSlaPolicies",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
