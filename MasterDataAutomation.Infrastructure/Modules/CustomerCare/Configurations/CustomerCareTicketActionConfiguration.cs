using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareTicketActionConfiguration
    : IEntityTypeConfiguration<CustomerCareTicketAction>
{
    public void Configure(EntityTypeBuilder<CustomerCareTicketAction> builder)
    {
        builder.ToTable("CustomerCareTicketActions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActionType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedByUserName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => x.FollowUpAt);
    }
}