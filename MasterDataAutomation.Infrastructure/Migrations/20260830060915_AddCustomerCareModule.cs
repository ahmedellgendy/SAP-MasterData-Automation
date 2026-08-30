using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MasterDataAutomation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCareModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerCareCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsQualityCategory = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareSubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareSubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCareSubCategories_CustomerCareCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CustomerCareCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareSlaPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TicketType = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SubCategoryId = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    FirstResponseMinutes = table.Column<int>(type: "int", nullable: false),
                    ResolutionMinutes = table.Column<int>(type: "int", nullable: false),
                    EscalationMinutes = table.Column<int>(type: "int", nullable: true),
                    UseBusinessHours = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareSlaPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCareSlaPolicies_CustomerCareCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CustomerCareCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCareSlaPolicies_CustomerCareSubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "CustomerCareSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CustomerAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SubCategoryId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedToUserId = table.Column<int>(type: "int", nullable: true),
                    AssignedDepartmentId = table.Column<int>(type: "int", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    SalesRepId = table.Column<int>(type: "int", nullable: true),
                    SupervisorId = table.Column<int>(type: "int", nullable: true),
                    SlaPolicyId = table.Column<int>(type: "int", nullable: true),
                    FirstResponseDueAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstRespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionDueAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EscalationDueAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EscalatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextFollowUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFirstResponseBreached = table.Column<bool>(type: "bit", nullable: false),
                    IsResolutionBreached = table.Column<bool>(type: "bit", nullable: false),
                    ResolutionSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReopenCount = table.Column<int>(type: "int", nullable: false),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCareTickets_CustomerCareCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CustomerCareCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCareTickets_CustomerCareDepartments_AssignedDepartmentId",
                        column: x => x.AssignedDepartmentId,
                        principalTable: "CustomerCareDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCareTickets_CustomerCareSlaPolicies_SlaPolicyId",
                        column: x => x.SlaPolicyId,
                        principalTable: "CustomerCareSlaPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerCareTickets_CustomerCareSubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "CustomerCareSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareAssignmentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<int>(type: "int", nullable: true),
                    ToUserId = table.Column<int>(type: "int", nullable: true),
                    FromDepartmentId = table.Column<int>(type: "int", nullable: true),
                    ToDepartmentId = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareAssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCareAssignmentHistories_CustomerCareTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "CustomerCareTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadedByUserId = table.Column<int>(type: "int", nullable: false),
                    UploadedByUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCareAttachments_CustomerCareTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "CustomerCareTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareQualityDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QualityIssueType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    QualityIssueDetails = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SampleRequired = table.Column<bool>(type: "bit", nullable: false),
                    SampleCollected = table.Column<bool>(type: "bit", nullable: false),
                    SampleCollectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QualityDecision = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CompensationType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CompensationQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CompensationNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    HasHealthRisk = table.Column<bool>(type: "bit", nullable: false),
                    HasLegalRisk = table.Column<bool>(type: "bit", nullable: false),
                    RiskNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareQualityDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCareQualityDetails_CustomerCareTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "CustomerCareTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: true),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCareStatusHistories_CustomerCareTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "CustomerCareTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCareTicketActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsInternalNote = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FollowUpAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCareTicketActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCareTicketActions_CustomerCareTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "CustomerCareTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CustomerCareCategories",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsActive", "IsQualityCategory", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "SALES", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Complaints related to sales representatives, supervisors and sales handling.", true, false, "Sales & Representative", 1 },
                    { 2, "DELIVERY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Complaints related to product supply, delivery and order fulfillment.", true, false, "Delivery & Supply", 2 },
                    { 3, "REPLACEMENT", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Complaints related to refused or delayed product replacement.", true, false, "Product Replacement", 3 },
                    { 4, "FREEZER", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Freezer requests, faults, maintenance and retrieval cases.", true, false, "Freezer & Maintenance", 4 },
                    { 5, "QUALITY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Product quality complaints and quality-related customer cases.", true, true, "Product Quality", 5 },
                    { 6, "COMMERCIAL", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "New customer, contracting, pricing and commercial service requests.", true, false, "Commercial Requests", 6 },
                    { 7, "GENERAL_INQUIRY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "General inquiries, information requests and call transfers.", true, false, "General Inquiry", 7 },
                    { 8, "OTHER", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Cases that do not currently match another category.", true, false, "Other", 99 }
                });

            migrationBuilder.InsertData(
                table: "CustomerCareDepartments",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsActive", "Name", "SortOrder", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "CUSTOMER_CARE", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer care and complaint handling team.", true, "Customer Care", 1, null },
                    { 2, "QUALITY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Quality assurance and product complaint handling.", true, "Quality", 2, null },
                    { 3, "SALES", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Sales representatives and sales-related complaint handling.", true, "Sales", 3, null },
                    { 4, "BRANCHES", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Branch-level operational complaint handling.", true, "Branches", 4, null },
                    { 5, "DISTRIBUTION", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Distribution and delivery-related complaint handling.", true, "Distribution", 5, null },
                    { 6, "MANAGEMENT", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Escalated cases requiring management intervention.", true, "Management", 6, null }
                });

            migrationBuilder.InsertData(
                table: "CustomerCareSubCategories",
                columns: new[] { "Id", "CategoryId", "Code", "CreatedAt", "Description", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, 1, "SALES_REP_COMPLAINT", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Complaint related to a sales representative.", true, "Sales Rep Complaint", 1 },
                    { 2, 1, "SUPERVISOR_COMPLAINT", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Complaint related to a sales supervisor.", true, "Supervisor Complaint", 2 },
                    { 3, 1, "SALES_REP_NO_RESPONSE", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer could not reach or receive a response from the sales representative.", true, "Sales Rep No Response", 3 },
                    { 10, 2, "NO_DELIVERY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Requested products were not delivered.", true, "No Delivery", 1 },
                    { 11, 2, "REFUSED_SUPPLY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Supply or product delivery was refused.", true, "Refused Supply", 2 },
                    { 12, 2, "INSUFFICIENT_QUANTITY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Delivered quantity did not meet the customer's requested quantity.", true, "Insufficient Quantity", 3 },
                    { 13, 2, "DELIVERY_DELAY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Delivery was delayed beyond the expected time.", true, "Delivery Delay", 4 },
                    { 20, 3, "REPLACEMENT_REFUSED", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Product replacement request was refused.", true, "Replacement Refused", 1 },
                    { 21, 3, "REPLACEMENT_DELAYED", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Product replacement was approved or requested but delayed.", true, "Replacement Delayed", 2 },
                    { 30, 4, "FREEZER_REQUEST", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer requested a company freezer.", true, "Freezer Request", 1 },
                    { 31, 4, "FREEZER_MAINTENANCE", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Freezer requires maintenance.", true, "Freezer Maintenance", 2 },
                    { 32, 4, "COOLING_FAILURE", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Freezer is not cooling or has stopped working.", true, "Cooling Failure", 3 },
                    { 33, 4, "FREEZER_RETRIEVAL", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer requested or requires freezer retrieval.", true, "Freezer Retrieval", 4 },
                    { 40, 5, "FOREIGN_OBJECT", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Foreign material was found inside the product.", true, "Foreign Object", 1 },
                    { 41, 5, "HAIR", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Hair was found in the product.", true, "Hair in Product", 2 },
                    { 42, 5, "PLASTIC", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Plastic material was found in the product.", true, "Plastic in Product", 3 },
                    { 43, 5, "METAL_OR_WIRE", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Metal or wire material was found in the product.", true, "Metal or Wire in Product", 4 },
                    { 44, 5, "TASTE_ISSUE", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer reported an abnormal or unacceptable taste.", true, "Taste Issue", 5 },
                    { 45, 5, "MELTED_PRODUCT", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Product was melted or affected by temperature handling.", true, "Melted Product", 6 },
                    { 46, 5, "ICE_CRYSTALS", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer reported abnormal ice crystals in the product.", true, "Ice Crystals", 7 },
                    { 47, 5, "PACKAGING_ISSUE", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Complaint related to product packaging.", true, "Packaging Issue", 8 },
                    { 48, 5, "MISSING_PRODUCTION_DATE", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Production or expiry information is missing or unclear.", true, "Missing Production Date", 9 },
                    { 49, 5, "MISSING_COMPONENT", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Expected product component or ingredient is missing.", true, "Missing Product Component", 10 },
                    { 50, 5, "OTHER_QUALITY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Other product quality complaint.", true, "Other Quality Issue", 99 },
                    { 60, 6, "NEW_CUSTOMER", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Request to register or contract a new customer.", true, "New Customer Request", 1 },
                    { 61, 6, "CONTRACT_REQUEST", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer requested contracting or commercial onboarding.", true, "Contract Request", 2 },
                    { 62, 6, "PRICING_INQUIRY", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer requested product or commercial pricing information.", true, "Pricing Inquiry", 3 },
                    { 70, 7, "GENERAL_INFORMATION", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "General customer inquiry.", true, "General Information", 1 },
                    { 71, 7, "CALL_TRANSFER", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Customer communication requires transfer to another party.", true, "Call Transfer", 2 },
                    { 80, 8, "OTHER", new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Case does not match an existing subcategory.", true, "Other", 99 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareAssignmentHistories_ChangedAt",
                table: "CustomerCareAssignmentHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareAssignmentHistories_TicketId",
                table: "CustomerCareAssignmentHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareAssignmentHistories_ToDepartmentId",
                table: "CustomerCareAssignmentHistories",
                column: "ToDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareAssignmentHistories_ToUserId",
                table: "CustomerCareAssignmentHistories",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareAttachments_TicketId",
                table: "CustomerCareAttachments",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareAttachments_UploadedAt",
                table: "CustomerCareAttachments",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareCategories_Code",
                table: "CustomerCareCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareDepartments_Code",
                table: "CustomerCareDepartments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareQualityDetails_ProductId",
                table: "CustomerCareQualityDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareQualityDetails_TicketId",
                table: "CustomerCareQualityDetails",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareSlaPolicies_CategoryId",
                table: "CustomerCareSlaPolicies",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareSlaPolicies_IsActive",
                table: "CustomerCareSlaPolicies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareSlaPolicies_SubCategoryId",
                table: "CustomerCareSlaPolicies",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareSlaPolicies_TicketType_CategoryId_SubCategoryId_Priority",
                table: "CustomerCareSlaPolicies",
                columns: new[] { "TicketType", "CategoryId", "SubCategoryId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareStatusHistories_ChangedAt",
                table: "CustomerCareStatusHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareStatusHistories_TicketId",
                table: "CustomerCareStatusHistories",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareSubCategories_CategoryId_Code",
                table: "CustomerCareSubCategories",
                columns: new[] { "CategoryId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTicketActions_CreatedAt",
                table: "CustomerCareTicketActions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTicketActions_FollowUpAt",
                table: "CustomerCareTicketActions",
                column: "FollowUpAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTicketActions_TicketId",
                table: "CustomerCareTicketActions",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_AssignedDepartmentId",
                table: "CustomerCareTickets",
                column: "AssignedDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_AssignedToUserId",
                table: "CustomerCareTickets",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_BranchId",
                table: "CustomerCareTickets",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_CategoryId",
                table: "CustomerCareTickets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_CreatedAt",
                table: "CustomerCareTickets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_CustomerId",
                table: "CustomerCareTickets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_CustomerPhone",
                table: "CustomerCareTickets",
                column: "CustomerPhone");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_NextFollowUpAt",
                table: "CustomerCareTickets",
                column: "NextFollowUpAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_Priority",
                table: "CustomerCareTickets",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_ResolutionDueAt",
                table: "CustomerCareTickets",
                column: "ResolutionDueAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_SlaPolicyId",
                table: "CustomerCareTickets",
                column: "SlaPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_Status",
                table: "CustomerCareTickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_SubCategoryId",
                table: "CustomerCareTickets",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCareTickets_TicketNumber",
                table: "CustomerCareTickets",
                column: "TicketNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerCareAssignmentHistories");

            migrationBuilder.DropTable(
                name: "CustomerCareAttachments");

            migrationBuilder.DropTable(
                name: "CustomerCareQualityDetails");

            migrationBuilder.DropTable(
                name: "CustomerCareStatusHistories");

            migrationBuilder.DropTable(
                name: "CustomerCareTicketActions");

            migrationBuilder.DropTable(
                name: "CustomerCareTickets");

            migrationBuilder.DropTable(
                name: "CustomerCareDepartments");

            migrationBuilder.DropTable(
                name: "CustomerCareSlaPolicies");

            migrationBuilder.DropTable(
                name: "CustomerCareSubCategories");

            migrationBuilder.DropTable(
                name: "CustomerCareCategories");
        }
    }
}
