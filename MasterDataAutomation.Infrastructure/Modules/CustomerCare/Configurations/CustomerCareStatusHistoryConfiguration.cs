using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareStatusHistoryConfiguration
    : IEntityTypeConfiguration<CustomerCareStatusHistory>
{
    public void Configure(EntityTypeBuilder<CustomerCareStatusHistory> builder)
    {
        builder.ToTable("CustomerCareStatusHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FromStatus)
            .HasConversion<int?>();

        builder.Property(x => x.ToStatus)
            .HasConversion<int>();

        builder.Property(x => x.Reason)
            .HasMaxLength(1000);

        builder.Property(x => x.ChangedByUserName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => x.ChangedAt);
    }
}