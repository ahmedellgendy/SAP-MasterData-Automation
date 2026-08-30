using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareAssignmentHistoryConfiguration
    : IEntityTypeConfiguration<CustomerCareAssignmentHistory>
{
    public void Configure(EntityTypeBuilder<CustomerCareAssignmentHistory> builder)
    {
        builder.ToTable("CustomerCareAssignmentHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .HasMaxLength(1000);

        builder.Property(x => x.ChangedByUserName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => x.ToUserId);

        builder.HasIndex(x => x.ToDepartmentId);

        builder.HasIndex(x => x.ChangedAt);
    }
}