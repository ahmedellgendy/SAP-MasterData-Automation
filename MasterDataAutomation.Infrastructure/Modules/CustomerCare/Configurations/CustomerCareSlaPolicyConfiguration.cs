using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareSlaPolicyConfiguration
    : IEntityTypeConfiguration<CustomerCareSlaPolicy>
{
    public void Configure(EntityTypeBuilder<CustomerCareSlaPolicy> builder)
    {
        builder.ToTable("CustomerCareSlaPolicies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.TicketType)
            .HasConversion<int?>();

        builder.Property(x => x.Priority)
            .HasConversion<int?>();

        builder.HasOne<CustomerCareCategory>()
    .WithMany()
    .HasForeignKey(x => x.CategoryId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CustomerCareSubCategory>()
            .WithMany()
            .HasForeignKey(x => x.SubCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.IsActive);

        builder.HasIndex(x => new
        {
            x.TicketType,
            x.CategoryId,
            x.SubCategoryId,
            x.Priority
        });

        builder.HasData(

    // =========================================================
    // DEFAULT COMPLAINT
    // First response: 2 hours
    // Resolution: 24 hours
    // Escalation: 8 hours
    // =========================================================

    new CustomerCareSlaPolicy
    {
        Id = 1,
        Name = "Default Complaint SLA",
        Description = "Default SLA for customer complaints.",
        TicketType = CustomerCareTicketType.Complaint,
        CategoryId = null,
        SubCategoryId = null,
        Priority = null,
        FirstResponseMinutes = 120,
        ResolutionMinutes = 1440,
        EscalationMinutes = 480,
        UseBusinessHours = false,
        IsActive = true,
        SortOrder = 100,
        CreatedAt = new DateTime(
            2026, 8, 30, 0, 0, 0,
            DateTimeKind.Utc)
    },

    // =========================================================
    // HIGH PRIORITY COMPLAINT
    // =========================================================

    new CustomerCareSlaPolicy
    {
        Id = 2,
        Name = "High Priority Complaint SLA",
        Description = "SLA for high-priority complaints.",
        TicketType = CustomerCareTicketType.Complaint,
        CategoryId = null,
        SubCategoryId = null,
        Priority = CustomerCareTicketPriority.High,
        FirstResponseMinutes = 60,
        ResolutionMinutes = 480,
        EscalationMinutes = 180,
        UseBusinessHours = false,
        IsActive = true,
        SortOrder = 20,
        CreatedAt = new DateTime(
            2026, 8, 30, 0, 0, 0,
            DateTimeKind.Utc)
    },

    // =========================================================
    // CRITICAL COMPLAINT
    // =========================================================

    new CustomerCareSlaPolicy
    {
        Id = 3,
        Name = "Critical Complaint SLA",
        Description = "SLA for critical customer complaints.",
        TicketType = CustomerCareTicketType.Complaint,
        CategoryId = null,
        SubCategoryId = null,
        Priority = CustomerCareTicketPriority.Critical,
        FirstResponseMinutes = 30,
        ResolutionMinutes = 240,
        EscalationMinutes = 60,
        UseBusinessHours = false,
        IsActive = true,
        SortOrder = 10,
        CreatedAt = new DateTime(
            2026, 8, 30, 0, 0, 0,
            DateTimeKind.Utc)
    },

    // =========================================================
    // QUALITY COMPLAINT
    // CategoryId = 5
    // =========================================================

    new CustomerCareSlaPolicy
    {
        Id = 4,
        Name = "Quality Complaint SLA",
        Description = "Default SLA for product quality complaints.",
        TicketType = CustomerCareTicketType.Complaint,
        CategoryId = 5,
        SubCategoryId = null,
        Priority = null,
        FirstResponseMinutes = 60,
        ResolutionMinutes = 480,
        EscalationMinutes = 120,
        UseBusinessHours = false,
        IsActive = true,
        SortOrder = 15,
        CreatedAt = new DateTime(
            2026, 8, 30, 0, 0, 0,
            DateTimeKind.Utc)
    },

    // =========================================================
    // CRITICAL QUALITY COMPLAINT
    // =========================================================

    new CustomerCareSlaPolicy
    {
        Id = 5,
        Name = "Critical Quality Complaint SLA",
        Description = "Critical SLA for quality complaints involving serious risk.",
        TicketType = CustomerCareTicketType.Complaint,
        CategoryId = 5,
        SubCategoryId = null,
        Priority = CustomerCareTicketPriority.Critical,
        FirstResponseMinutes = 15,
        ResolutionMinutes = 180,
        EscalationMinutes = 30,
        UseBusinessHours = false,
        IsActive = true,
        SortOrder = 1,
        CreatedAt = new DateTime(
            2026, 8, 30, 0, 0, 0,
            DateTimeKind.Utc)
    },

    // =========================================================
    // REQUEST
    // =========================================================

    new CustomerCareSlaPolicy
    {
        Id = 6,
        Name = "Customer Request SLA",
        Description = "Default SLA for customer requests.",
        TicketType = CustomerCareTicketType.Request,
        CategoryId = null,
        SubCategoryId = null,
        Priority = null,
        FirstResponseMinutes = 240,
        ResolutionMinutes = 2880,
        EscalationMinutes = 1440,
        UseBusinessHours = false,
        IsActive = true,
        SortOrder = 100,
        CreatedAt = new DateTime(
            2026, 8, 30, 0, 0, 0,
            DateTimeKind.Utc)
    },

    // =========================================================
    // INQUIRY
    // =========================================================

    new CustomerCareSlaPolicy
    {
        Id = 7,
        Name = "Customer Inquiry SLA",
        Description = "Default SLA for customer inquiries.",
        TicketType = CustomerCareTicketType.Inquiry,
        CategoryId = null,
        SubCategoryId = null,
        Priority = null,
        FirstResponseMinutes = 120,
        ResolutionMinutes = 480,
        EscalationMinutes = null,
        UseBusinessHours = false,
        IsActive = true,
        SortOrder = 100,
        CreatedAt = new DateTime(
            2026, 8, 30, 0, 0, 0,
            DateTimeKind.Utc)
    },

    // =========================================================
    // TRANSFER
    // =========================================================

    new CustomerCareSlaPolicy
    {
        Id = 8,
        Name = "Transfer SLA",
        Description = "Default SLA for transferred customer cases.",
        TicketType = CustomerCareTicketType.Transfer,
        CategoryId = null,
        SubCategoryId = null,
        Priority = null,
        FirstResponseMinutes = 60,
        ResolutionMinutes = 240,
        EscalationMinutes = 120,
        UseBusinessHours = false,
        IsActive = true,
        SortOrder = 100,
        CreatedAt = new DateTime(
            2026, 8, 30, 0, 0, 0,
            DateTimeKind.Utc)
    }
);
    }
}