using MasterDataAutomation.Infrastructure.Data.Entities;
using MasterDataAutomation.Infrastructure.Data.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DraftCustomerEntity> DraftCustomers { get; set; }

    public DbSet<GenerationHistoryEntity> GenerationHistories { get; set; }

    public DbSet<SystemSettingEntity> SystemSettings { get; set; }
    public DbSet<AppUserEntity> AppUsers { get; set; }

    public DbSet<ProductEntity> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DraftCustomerEntity>(entity =>
        {
            entity.ToTable("DraftCustomers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Line)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Market)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Branch)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.SalesDistrict)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.CustomerType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Draft");

            entity.Property(x => x.RejectionReason)
                .HasMaxLength(500);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<GenerationHistoryEntity>(entity =>
        {
            entity.ToTable("GenerationHistories");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);
        });

        modelBuilder.Entity<SystemSettingEntity>(entity =>
        {
            entity.ToTable("SystemSettings");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasIndex(x => x.Key)
                .IsUnique();
        });

        modelBuilder.Entity<AppUserEntity>(entity =>
        {
            entity.ToTable("AppUsers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.UserName)
                .IsUnique();
        });

        // Product Master
        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.ItemName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.MarketSellingPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.RetailSellingPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.Currency)
                .HasMaxLength(10);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.ItemCode)
                .IsUnique();
        });
    }
}