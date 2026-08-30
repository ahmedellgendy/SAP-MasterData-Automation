using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareTicketGiftConfiguration
    : IEntityTypeConfiguration<CustomerCareTicketGift>
{
    public void Configure(
        EntityTypeBuilder<CustomerCareTicketGift> builder)
    {
        builder.ToTable("CustomerCareTicketGifts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.RecipientName)
            .HasMaxLength(200);

        builder.Property(x => x.RecipientPhone)
            .HasMaxLength(50);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedByUserName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Gifts)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => x.CreatedAt);
    }
}