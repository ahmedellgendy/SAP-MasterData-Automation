using MasterDataAutomation.Infrastructure.Data.Entities;
using MasterDataAutomation.Infrastructure.Data.Entities.Customers;
using MasterDataAutomation.Infrastructure.Data.Entities.Products;
using MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;
using MasterDataAutomation.Infrastructure.Data.Entities.System;
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

    // Product Master
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductRequestEntity> ProductRequests { get; set; }

    public DbSet<ActivityLogEntity> ActivityLogs { get; set; }

    public DbSet<CustomerModificationRequestEntity> CustomerModificationRequests { get; set; }

    // Data Analytics Dbsets
    public DbSet<SalesAnalyticsUploadBatchEntity> SalesAnalyticsUploadBatches { get; set; }
    public DbSet<SalesAnalyticsCustomerEntity> SalesAnalyticsCustomers { get; set; }
    public DbSet<SalesAnalyticsSalesRepEntity> SalesAnalyticsSalesReps { get; set; }
    public DbSet<SalesRepRouteAssignmentEntity> SalesRepRouteAssignments { get; set; }
    public DbSet<SalesAnalyticsDailySalesReportEntity> SalesAnalyticsDailySalesReports { get; set; }
    public DbSet<SalesAnalyticsDailyVisitReportEntity> SalesAnalyticsDailyVisitReports { get; set; }
    public DbSet<SalesDistrictMonthlyTargetEntity> SalesDistrictMonthlyTargets { get; set; }
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

            entity.Property(x => x.CreatedBy)
                 .HasMaxLength(150);
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

            entity.Property(x => x.OutletSellingPrice)
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
        // Product Requests
        modelBuilder.Entity<ProductRequestEntity>(entity =>
        {
            entity.ToTable("ProductRequests");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ItemCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.ItemName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.Notes)
                .HasMaxLength(500);

            entity.Property(x => x.PurchasePrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.PiecePrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.OutletSellingPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.RetailSellingPrice)
                .HasPrecision(18, 2);

            entity.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.RejectionReason)
                .HasMaxLength(500);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.ItemCode);
        });

        modelBuilder.Entity<ActivityLogEntity>(entity =>
        {
            entity.ToTable("ActivityLogs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserName)
                .HasMaxLength(100);

            entity.Property(x => x.UserRole)
                .HasMaxLength(100);

            entity.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.Module)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<CustomerModificationRequestEntity>(entity =>
        {
            entity.ToTable("CustomerModificationRequests");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.BranchName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.MarketCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.MarketName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.NewMarketName)
                .HasMaxLength(250);

            entity.Property(x => x.Notes)
                .HasMaxLength(500);

            entity.Property(x => x.RejectionReason)
                .HasMaxLength(500);

            entity.Property(x => x.CreatedBy)
                .HasMaxLength(150);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.MarketCode);
            entity.HasIndex(x => x.Status);
        });

        // Sales Analytics
        modelBuilder.Entity<SalesAnalyticsUploadBatchEntity>(entity =>
        {
            entity.ToTable("SalesAnalyticsUploadBatches");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.OriginalFileName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.UploadedBy)
                .HasMaxLength(150);

            entity.Property(x => x.ErrorMessage)
                .HasMaxLength(1000);

            entity.Property(x => x.UploadDate)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.FileType);
            entity.HasIndex(x => x.ReportDate);
            entity.HasIndex(x => x.UploadDate);
        });
        modelBuilder.Entity<SalesAnalyticsCustomerEntity>(entity =>
        {
            entity.ToTable("SalesAnalyticsCustomers");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.CustomerCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.CustomerAccountGroup)
                .HasMaxLength(50);

            entity.Property(x => x.BranchCode)
                .HasMaxLength(50);

            entity.Property(x => x.BranchName)
                .HasMaxLength(150);

            entity.Property(x => x.SalesDistrictCode)
                .HasMaxLength(50);

            entity.Property(x => x.SalesDistrictName)
                .HasMaxLength(150);

            entity.Property(x => x.CustomerClassificationCode)
                .HasMaxLength(50);

            entity.Property(x => x.CustomerClassificationName)
                .HasMaxLength(150);

            entity.Property(x => x.IncotermsCode)
                .HasMaxLength(50);

            entity.Property(x => x.IncotermsName)
                .HasMaxLength(150);

            entity.Property(x => x.SearchTerm)
                .HasMaxLength(150);

            entity.Property(x => x.SearchTerm2)
                .HasMaxLength(150);

            entity.Property(x => x.SourceCreatedBy)
                .HasMaxLength(150);

            entity.Property(x => x.ImportedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.CustomerCode)
                .IsUnique();

            entity.HasIndex(x => x.BranchCode);
            entity.HasIndex(x => x.SalesDistrictCode);
            entity.HasIndex(x => x.UploadBatchId);
        });
        modelBuilder.Entity<SalesAnalyticsSalesRepEntity>(entity =>
        {
            entity.ToTable("SalesAnalyticsSalesReps");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.SalesRepCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.SalesRepName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.BranchCode)
                .HasMaxLength(50);

            entity.Property(x => x.BranchName)
                .HasMaxLength(150);

            entity.Property(x => x.RegionCode)
                .HasMaxLength(50);

            entity.Property(x => x.RegionName)
                .HasMaxLength(150);

            entity.Property(x => x.InternalCode)
                .HasMaxLength(100);

            entity.Property(x => x.SearchTerm2)
                .HasMaxLength(150);

            entity.Property(x => x.SourceCreatedBy)
                .HasMaxLength(150);

            entity.Property(x => x.ImportedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.SalesRepCode)
                .IsUnique();

            entity.HasIndex(x => x.BranchCode);
            entity.HasIndex(x => x.InternalCode);
            entity.HasIndex(x => x.UploadBatchId);
        }); 
        modelBuilder.Entity<SalesRepRouteAssignmentEntity>(entity =>
        {
            entity.ToTable("SalesRepRouteAssignments");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.SalesRepCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.SalesRepName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.SalesDistrictCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.SalesDistrictName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.BranchCode)
                .HasMaxLength(50);

            entity.Property(x => x.BranchName)
                .HasMaxLength(150);

            entity.Property(x => x.ImportedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.SalesRepCode);
            entity.HasIndex(x => x.SalesDistrictCode);
            entity.HasIndex(x => x.BranchCode);
            entity.HasIndex(x => x.IsActive);
            entity.HasIndex(x => x.UploadBatchId);
        });
        modelBuilder.Entity<SalesAnalyticsDailySalesReportEntity>(entity =>
        {
            entity.ToTable("SalesAnalyticsDailySalesReports");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.LineCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.LineName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.ProductName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.Quantity)
                .HasPrecision(18, 3);

            entity.Property(x => x.SalesAmount)
                .HasPrecision(18, 2);

            entity.Property(x => x.ImportedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.ReportDate);
            entity.HasIndex(x => x.LineCode);
            entity.HasIndex(x => x.ProductName);
            entity.HasIndex(x => x.UploadBatchId);
        });
        modelBuilder.Entity<SalesAnalyticsDailyVisitReportEntity>(entity =>
        {
            entity.ToTable("SalesAnalyticsDailyVisitReports");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.SupervisorName)
                .HasMaxLength(150);

            entity.Property(x => x.CityName)
                .HasMaxLength(150);

            entity.Property(x => x.VisitCode)
                .HasMaxLength(100);

            entity.Property(x => x.SalesRepCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.SalesRepName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.CustomerCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.VisitStatus)
                .HasMaxLength(100);

            entity.Property(x => x.NegativeReason)
                .HasMaxLength(250);

            entity.Property(x => x.SuccessfulVisitValue)
                .HasPrecision(18, 2);

            entity.Property(x => x.VisitDurationText)
                .HasMaxLength(100);

            entity.Property(x => x.ImportedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => x.ReportDate);
            entity.HasIndex(x => x.SalesRepCode);
            entity.HasIndex(x => x.CustomerCode);
            entity.HasIndex(x => x.VisitStatus);
            entity.HasIndex(x => x.UploadBatchId);
        });
        modelBuilder.Entity<SalesDistrictMonthlyTargetEntity>(entity =>
        {
            entity.ToTable("SalesDistrictMonthlyTargets");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.BranchCode)
                .HasMaxLength(50);

            entity.Property(x => x.BranchName)
                .HasMaxLength(150);

            entity.Property(x => x.SalesDistrictCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.SalesDistrictName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.MonthlySalesTarget)
                .HasPrecision(18, 2);

            entity.Property(x => x.CreatedBy)
                .HasMaxLength(150);

            entity.Property(x => x.UpdatedBy)
                .HasMaxLength(150);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasIndex(x => new
            {
                x.Year,
                x.Month,
                x.SalesDistrictCode
            }).IsUnique();

            entity.HasIndex(x => x.BranchCode);
            entity.HasIndex(x => x.SalesDistrictCode);
        });
    }
}