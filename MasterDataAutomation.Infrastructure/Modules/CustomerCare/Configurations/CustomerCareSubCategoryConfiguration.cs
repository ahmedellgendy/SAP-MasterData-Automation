using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareSubCategoryConfiguration
    : IEntityTypeConfiguration<CustomerCareSubCategory>
{
    public void Configure(EntityTypeBuilder<CustomerCareSubCategory> builder)
    {
        builder.ToTable("CustomerCareSubCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasIndex(x => new
        {
            x.CategoryId,
            x.Code
        })
        .IsUnique();


        builder.HasData(

// =========================================================
// SALES - CategoryId = 1
// =========================================================

new CustomerCareSubCategory
{
    Id = 1,
    CategoryId = 1,
    Code = "SALES_REP_COMPLAINT",
    Name = "Sales Rep Complaint",
    Description = "Complaint related to a sales representative.",
    IsActive = true,
    SortOrder = 1,
    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 2,
CategoryId = 1,
Code = "SUPERVISOR_COMPLAINT",
Name = "Supervisor Complaint",
Description = "Complaint related to a sales supervisor.",
IsActive = true,
SortOrder = 2,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 3,
CategoryId = 1,
Code = "SALES_REP_NO_RESPONSE",
Name = "Sales Rep No Response",
Description = "Customer could not reach or receive a response from the sales representative.",
IsActive = true,
SortOrder = 3,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},

// =========================================================
// DELIVERY - CategoryId = 2
// =========================================================

new CustomerCareSubCategory
{
    Id = 10,
    CategoryId = 2,
    Code = "NO_DELIVERY",
    Name = "No Delivery",
    Description = "Requested products were not delivered.",
    IsActive = true,
    SortOrder = 1,
    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 11,
CategoryId = 2,
Code = "REFUSED_SUPPLY",
Name = "Refused Supply",
Description = "Supply or product delivery was refused.",
IsActive = true,
SortOrder = 2,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 12,
CategoryId = 2,
Code = "INSUFFICIENT_QUANTITY",
Name = "Insufficient Quantity",
Description = "Delivered quantity did not meet the customer's requested quantity.",
IsActive = true,
SortOrder = 3,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 13,
CategoryId = 2,
Code = "DELIVERY_DELAY",
Name = "Delivery Delay",
Description = "Delivery was delayed beyond the expected time.",
IsActive = true,
SortOrder = 4,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},

// =========================================================
// REPLACEMENT - CategoryId = 3
// =========================================================

new CustomerCareSubCategory
{
    Id = 20,
    CategoryId = 3,
    Code = "REPLACEMENT_REFUSED",
    Name = "Replacement Refused",
    Description = "Product replacement request was refused.",
    IsActive = true,
    SortOrder = 1,
    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 21,
CategoryId = 3,
Code = "REPLACEMENT_DELAYED",
Name = "Replacement Delayed",
Description = "Product replacement was approved or requested but delayed.",
IsActive = true,
SortOrder = 2,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},

// =========================================================
// FREEZER - CategoryId = 4
// =========================================================

new CustomerCareSubCategory
{
    Id = 30,
    CategoryId = 4,
    Code = "FREEZER_REQUEST",
    Name = "Freezer Request",
    Description = "Customer requested a company freezer.",
    IsActive = true,
    SortOrder = 1,
    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 31,
CategoryId = 4,
Code = "FREEZER_MAINTENANCE",
Name = "Freezer Maintenance",
Description = "Freezer requires maintenance.",
IsActive = true,
SortOrder = 2,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 32,
CategoryId = 4,
Code = "COOLING_FAILURE",
Name = "Cooling Failure",
Description = "Freezer is not cooling or has stopped working.",
IsActive = true,
SortOrder = 3,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 33,
CategoryId = 4,
Code = "FREEZER_RETRIEVAL",
Name = "Freezer Retrieval",
Description = "Customer requested or requires freezer retrieval.",
IsActive = true,
SortOrder = 4,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},

// =========================================================
// QUALITY - CategoryId = 5
// =========================================================

new CustomerCareSubCategory
{
    Id = 40,
    CategoryId = 5,
    Code = "FOREIGN_OBJECT",
    Name = "Foreign Object",
    Description = "Foreign material was found inside the product.",
    IsActive = true,
    SortOrder = 1,
    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 41,
CategoryId = 5,
Code = "HAIR",
Name = "Hair in Product",
Description = "Hair was found in the product.",
IsActive = true,
SortOrder = 2,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 42,
CategoryId = 5,
Code = "PLASTIC",
Name = "Plastic in Product",
Description = "Plastic material was found in the product.",
IsActive = true,
SortOrder = 3,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 43,
CategoryId = 5,
Code = "METAL_OR_WIRE",
Name = "Metal or Wire in Product",
Description = "Metal or wire material was found in the product.",
IsActive = true,
SortOrder = 4,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 44,
CategoryId = 5,
Code = "TASTE_ISSUE",
Name = "Taste Issue",
Description = "Customer reported an abnormal or unacceptable taste.",
IsActive = true,
SortOrder = 5,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 45,
CategoryId = 5,
Code = "MELTED_PRODUCT",
Name = "Melted Product",
Description = "Product was melted or affected by temperature handling.",
IsActive = true,
SortOrder = 6,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 46,
CategoryId = 5,
Code = "ICE_CRYSTALS",
Name = "Ice Crystals",
Description = "Customer reported abnormal ice crystals in the product.",
IsActive = true,
SortOrder = 7,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 47,
CategoryId = 5,
Code = "PACKAGING_ISSUE",
Name = "Packaging Issue",
Description = "Complaint related to product packaging.",
IsActive = true,
SortOrder = 8,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 48,
CategoryId = 5,
Code = "MISSING_PRODUCTION_DATE",
Name = "Missing Production Date",
Description = "Production or expiry information is missing or unclear.",
IsActive = true,
SortOrder = 9,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 49,
CategoryId = 5,
Code = "MISSING_COMPONENT",
Name = "Missing Product Component",
Description = "Expected product component or ingredient is missing.",
IsActive = true,
SortOrder = 10,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 50,
CategoryId = 5,
Code = "OTHER_QUALITY",
Name = "Other Quality Issue",
Description = "Other product quality complaint.",
IsActive = true,
SortOrder = 99,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},

// =========================================================
// COMMERCIAL - CategoryId = 6
// =========================================================

new CustomerCareSubCategory
{
    Id = 60,
    CategoryId = 6,
    Code = "NEW_CUSTOMER",
    Name = "New Customer Request",
    Description = "Request to register or contract a new customer.",
    IsActive = true,
    SortOrder = 1,
    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 61,
CategoryId = 6,
Code = "CONTRACT_REQUEST",
Name = "Contract Request",
Description = "Customer requested contracting or commercial onboarding.",
IsActive = true,
SortOrder = 2,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 62,
CategoryId = 6,
Code = "PRICING_INQUIRY",
Name = "Pricing Inquiry",
Description = "Customer requested product or commercial pricing information.",
IsActive = true,
SortOrder = 3,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},

// =========================================================
// GENERAL INQUIRY - CategoryId = 7
// =========================================================

new CustomerCareSubCategory
{
    Id = 70,
    CategoryId = 7,
    Code = "GENERAL_INFORMATION",
    Name = "General Information",
    Description = "General customer inquiry.",
    IsActive = true,
    SortOrder = 1,
    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},
new CustomerCareSubCategory
{
Id = 71,
CategoryId = 7,
Code = "CALL_TRANSFER",
Name = "Call Transfer",
Description = "Customer communication requires transfer to another party.",
IsActive = true,
SortOrder = 2,
CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
},

// =========================================================
// OTHER - CategoryId = 8
// =========================================================

new CustomerCareSubCategory
{
    Id = 80,
    CategoryId = 8,
    Code = "OTHER",
    Name = "Other",
    Description = "Case does not match an existing subcategory.",
    IsActive = true,
    SortOrder = 99,
    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
}
);

    }
}