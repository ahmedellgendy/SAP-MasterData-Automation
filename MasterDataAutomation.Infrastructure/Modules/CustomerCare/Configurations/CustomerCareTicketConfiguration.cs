using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareTicketConfiguration
    : IEntityTypeConfiguration<CustomerCareTicket>
{
    public void Configure(EntityTypeBuilder<CustomerCareTicket> builder)
    {
        builder.ToTable("CustomerCareTickets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TicketNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.TicketNumber)
            .IsUnique();

        builder.Property(x => x.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CustomerPhone)
            .HasMaxLength(30);

        builder.Property(x => x.CustomerAddress)
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.ResolutionSummary)
            .HasMaxLength(4000);

        builder.Property(x => x.Source)
            .HasConversion<int>();

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.Priority)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        // =====================================================
        // Category
        // =====================================================

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SubCategory)
            .WithMany()
            .HasForeignKey(x => x.SubCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // Department
        // =====================================================

        builder.HasOne(x => x.AssignedDepartment)
            .WithMany()
            .HasForeignKey(x => x.AssignedDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // SLA
        // =====================================================

        builder.HasOne(x => x.SlaPolicy)
            .WithMany()
            .HasForeignKey(x => x.SlaPolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================================
        // Quality Detail
        // =====================================================

        builder.HasOne(x => x.QualityDetail)
            .WithOne(x => x.Ticket)
            .HasForeignKey<CustomerCareQualityDetail>(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // =====================================================
        // Actions
        // =====================================================

        builder.HasMany(x => x.Actions)
            .WithOne(x => x.Ticket)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // =====================================================
        // Attachments
        // =====================================================

        builder.HasMany(x => x.Attachments)
            .WithOne(x => x.Ticket)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // =====================================================
        // Assignment History
        // =====================================================

        builder.HasMany(x => x.AssignmentHistory)
            .WithOne(x => x.Ticket)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // =====================================================
        // Status History
        // =====================================================

        builder.HasMany(x => x.StatusHistory)
            .WithOne(x => x.Ticket)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // =====================================================
        // Indexes
        // =====================================================

        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => x.CustomerPhone);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Priority);

        builder.HasIndex(x => x.AssignedToUserId);

        builder.HasIndex(x => x.AssignedDepartmentId);

        builder.HasIndex(x => x.BranchId);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => x.ResolutionDueAt);

        builder.HasIndex(x => x.NextFollowUpAt);
    }
}