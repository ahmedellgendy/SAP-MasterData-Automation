using MasterDataAutomation.Domain.Modules.CustomerCare.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasterDataAutomation.Infrastructure.Modules.CustomerCare.Configurations;

public class CustomerCareAttachmentConfiguration
    : IEntityTypeConfiguration<CustomerCareAttachment>
{
    public void Configure(EntityTypeBuilder<CustomerCareAttachment> builder)
    {
        builder.ToTable("CustomerCareAttachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StoredFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.FilePath)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(150);

        builder.Property(x => x.AttachmentType)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.UploadedByUserName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => x.UploadedAt);
    }
}