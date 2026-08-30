using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareCategoryConfiguration
    : IEntityTypeConfiguration<CustomerCareCategory>
{
    public void Configure(EntityTypeBuilder<CustomerCareCategory> builder)
    {
        builder.ToTable("CustomerCareCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasMany(x => x.SubCategories)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
                new CustomerCareCategory
                {
                    Id = 1,
                    Code = "SALES",
                    Name = "Sales & Representative",
                    Description = "Complaints related to sales representatives, supervisors and sales handling.",
                    IsQualityCategory = false,
                    IsActive = true,
                    SortOrder = 1,
                    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
                },
                new CustomerCareCategory
                {
                    Id = 2,
                    Code = "DELIVERY",
                    Name = "Delivery & Supply",
                    Description = "Complaints related to product supply, delivery and order fulfillment.",
                    IsQualityCategory = false,
                    IsActive = true,
                    SortOrder = 2,
                    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
                },
                new CustomerCareCategory
                {
                    Id = 3,
                    Code = "REPLACEMENT",
                    Name = "Product Replacement",
                    Description = "Complaints related to refused or delayed product replacement.",
                    IsQualityCategory = false,
                    IsActive = true,
                    SortOrder = 3,
                    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
                },
                new CustomerCareCategory
                {
                    Id = 4,
                    Code = "FREEZER",
                    Name = "Freezer & Maintenance",
                    Description = "Freezer requests, faults, maintenance and retrieval cases.",
                    IsQualityCategory = false,
                    IsActive = true,
                    SortOrder = 4,
                    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
                },
                new CustomerCareCategory
                {
                    Id = 5,
                    Code = "QUALITY",
                    Name = "Product Quality",
                    Description = "Product quality complaints and quality-related customer cases.",
                    IsQualityCategory = true,
                    IsActive = true,
                    SortOrder = 5,
                    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
                },
                new CustomerCareCategory
                {
                    Id = 6,
                    Code = "COMMERCIAL",
                    Name = "Commercial Requests",
                    Description = "New customer, contracting, pricing and commercial service requests.",
                    IsQualityCategory = false,
                    IsActive = true,
                    SortOrder = 6,
                    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
                },
                new CustomerCareCategory
                {
                    Id = 7,
                    Code = "GENERAL_INQUIRY",
                    Name = "General Inquiry",
                    Description = "General inquiries, information requests and call transfers.",
                    IsQualityCategory = false,
                    IsActive = true,
                    SortOrder = 7,
                    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
                },
                new CustomerCareCategory
                {
                    Id = 8,
                    Code = "OTHER",
                    Name = "Other",
                    Description = "Cases that do not currently match another category.",
                    IsQualityCategory = false,
                    IsActive = true,
                    SortOrder = 99,
                    CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
                }
                );
    }
}