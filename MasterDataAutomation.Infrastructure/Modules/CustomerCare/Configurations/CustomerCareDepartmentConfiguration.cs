using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareDepartmentConfiguration
    : IEntityTypeConfiguration<CustomerCareDepartment>
{
    public void Configure(EntityTypeBuilder<CustomerCareDepartment> builder)
    {
        builder.ToTable("CustomerCareDepartments");

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

        builder.HasData(
          new CustomerCareDepartment
          {
              Id = 1,
              Code = "CUSTOMER_CARE",
              Name = "Customer Care",
              Description = "Customer care and complaint handling team.",
              IsActive = true,
              SortOrder = 1,
              CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
          },
          new CustomerCareDepartment
          {
              Id = 2,
              Code = "QUALITY",
              Name = "Quality",
              Description = "Quality assurance and product complaint handling.",
              IsActive = true,
              SortOrder = 2,
              CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
          },
          new CustomerCareDepartment
          {
              Id = 3,
              Code = "SALES",
              Name = "Sales",
              Description = "Sales representatives and sales-related complaint handling.",
              IsActive = true,
              SortOrder = 3,
              CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
          },
          new CustomerCareDepartment
          {
              Id = 4,
              Code = "BRANCHES",
              Name = "Branches",
              Description = "Branch-level operational complaint handling.",
              IsActive = true,
              SortOrder = 4,
              CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
          },
          new CustomerCareDepartment
          {
              Id = 5,
              Code = "DISTRIBUTION",
              Name = "Distribution",
              Description = "Distribution and delivery-related complaint handling.",
              IsActive = true,
              SortOrder = 5,
              CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
          },
          new CustomerCareDepartment
          {
              Id = 6,
              Code = "MANAGEMENT",
              Name = "Management",
              Description = "Escalated cases requiring management intervention.",
              IsActive = true,
              SortOrder = 6,
              CreatedAt = new DateTime(2026, 8, 30, 0, 0, 0, DateTimeKind.Utc)
          }
        );
    }
}