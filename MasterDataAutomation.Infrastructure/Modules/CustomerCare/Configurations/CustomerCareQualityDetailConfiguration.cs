using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareQualityDetailConfiguration
    : IEntityTypeConfiguration<CustomerCareQualityDetail>
{
    public void Configure(EntityTypeBuilder<CustomerCareQualityDetail> builder)
    {
        builder.ToTable("CustomerCareQualityDetails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductName)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.BatchNumber)
            .HasMaxLength(100);

        builder.Property(x => x.QualityIssueType)
            .HasMaxLength(150);

        builder.Property(x => x.QualityIssueDetails)
            .HasMaxLength(4000);

        builder.Property(x => x.QualityDecision)
            .HasMaxLength(4000);

        builder.Property(x => x.CompensationType)
            .HasMaxLength(150);

        builder.Property(x => x.CompensationNotes)
            .HasMaxLength(2000);

        builder.Property(x => x.RiskNotes)
            .HasMaxLength(2000);

        builder.Property(x => x.CompensationQuantity)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.TicketId)
            .IsUnique();

        builder.HasIndex(x => x.ProductId);
    }
}