using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Enums;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Persistence;

public class SalesAnalyticsMasterDataRepository : ISalesAnalyticsMasterDataRepository
{
    private readonly ApplicationDbContext _context;

    public SalesAnalyticsMasterDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public int CreateUploadBatch(SalesAnalyticsUploadFileType fileType,string originalFileName,DateTime? reportDate,string? uploadedBy)
    {
        var batch = new SalesAnalyticsUploadBatchEntity
        {
            FileType = fileType,
            Status = SalesAnalyticsUploadStatus.Success,
            OriginalFileName = originalFileName,
            ReportDate = reportDate,
            UploadedBy = uploadedBy,
            UploadDate = DateTime.Now
        };

        _context.SalesAnalyticsUploadBatches.Add(batch);
        _context.SaveChanges();

        return batch.Id;
    }

    public void CompleteUploadBatch(int uploadBatchId,SalesAnalyticsUploadStatus status,int totalRows,int importedRows,int failedRows,string? errorMessage = null)
    {
        var batch = _context.SalesAnalyticsUploadBatches
            .FirstOrDefault(x => x.Id == uploadBatchId);

        if (batch == null)
            return;

        batch.Status = status;
        batch.TotalRows = totalRows;
        batch.ImportedRows = importedRows;
        batch.FailedRows = failedRows;
        batch.ErrorMessage = errorMessage;

        _context.SaveChanges();
    }

    public void ReplaceCustomers(List<SalesAnalyticsCustomerImportDto> customers, int uploadBatchId)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var oldCustomers = _context.SalesAnalyticsCustomers.ToList();

            if (oldCustomers.Any())
            {
                _context.SalesAnalyticsCustomers.RemoveRange(oldCustomers);
                _context.SaveChanges();
            }

            var entities = customers.Select(x => new SalesAnalyticsCustomerEntity
            {
                CustomerCode = x.CustomerCode.Trim(),
                CustomerName = x.CustomerName.Trim(),

                CustomerAccountGroup = x.CustomerAccountGroup?.Trim(),

                BranchCode = x.BranchCode?.Trim(),
                BranchName = x.BranchName?.Trim(),

                SalesDistrictCode = x.SalesDistrictCode?.Trim(),
                SalesDistrictName = x.SalesDistrictName?.Trim(),

                CustomerClassificationCode = x.CustomerClassificationCode?.Trim(),
                CustomerClassificationName = x.CustomerClassificationName?.Trim(),

                IncotermsCode = x.IncotermsCode?.Trim(),
                IncotermsName = x.IncotermsName?.Trim(),

                SearchTerm = x.SearchTerm?.Trim(),
                SearchTerm2 = x.SearchTerm2?.Trim(),

                SourceCreatedDate = x.SourceCreatedDate,
                SourceCreatedBy = x.SourceCreatedBy?.Trim(),

                ImportedAt = DateTime.Now,
                UploadBatchId = uploadBatchId
            }).ToList();

            _context.SalesAnalyticsCustomers.AddRange(entities);
            _context.SaveChanges();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void ReplaceSalesReps(List<SalesAnalyticsSalesRepImportDto> salesReps, int uploadBatchId)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var oldSalesReps = _context.SalesAnalyticsSalesReps.ToList();

            if (oldSalesReps.Any())
            {
                _context.SalesAnalyticsSalesReps.RemoveRange(oldSalesReps);
                _context.SaveChanges();
            }

            var entities = salesReps.Select(x => new SalesAnalyticsSalesRepEntity
            {
                SalesRepCode = x.SalesRepCode.Trim(),
                SalesRepName = x.SalesRepName.Trim(),

                BranchCode = x.BranchCode?.Trim(),
                BranchName = x.BranchName?.Trim(),

                RegionCode = x.RegionCode?.Trim(),
                RegionName = x.RegionName?.Trim(),

                InternalCode = x.InternalCode?.Trim(),
                SearchTerm2 = x.SearchTerm2?.Trim(),

                SourceCreatedDate = x.SourceCreatedDate,
                SourceCreatedBy = x.SourceCreatedBy?.Trim(),

                ImportedAt = DateTime.Now,
                UploadBatchId = uploadBatchId
            }).ToList();

            _context.SalesAnalyticsSalesReps.AddRange(entities);
            _context.SaveChanges();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void ReplaceRepRouteAssignments(List<SalesRepRouteAssignmentImportDto> assignments, int uploadBatchId)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var oldAssignments = _context.SalesRepRouteAssignments.ToList();

            if (oldAssignments.Any())
            {
                _context.SalesRepRouteAssignments.RemoveRange(oldAssignments);
                _context.SaveChanges();
            }

            var entities = assignments.Select(x => new SalesRepRouteAssignmentEntity
            {
                SalesRepCode = x.SalesRepCode.Trim(),
                SalesRepName = x.SalesRepName.Trim(),

                SalesDistrictCode = x.SalesDistrictCode.Trim(),
                SalesDistrictName = x.SalesDistrictName.Trim(),

                BranchCode = x.BranchCode?.Trim(),
                BranchName = x.BranchName?.Trim(),

                EffectiveFrom = x.EffectiveFrom,
                EffectiveTo = x.EffectiveTo,

                IsActive = x.IsActive,
                ImportedAt = DateTime.Now,
                UploadBatchId = uploadBatchId
            }).ToList();

            _context.SalesRepRouteAssignments.AddRange(entities);
            _context.SaveChanges();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void ReplaceDailySalesReport(List<SalesAnalyticsDailySalesImportDto> salesRows,int uploadBatchId,DateTime reportDate)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var oldRows = _context.SalesAnalyticsDailySalesReports
                .Where(x => x.ReportDate.Date == reportDate.Date)
                .ToList();

            if (oldRows.Any())
            {
                _context.SalesAnalyticsDailySalesReports.RemoveRange(oldRows);
                _context.SaveChanges();
            }

            var entities = salesRows.Select(x => new SalesAnalyticsDailySalesReportEntity
            {
                ReportDate = reportDate.Date,

                LineCode = x.LineCode.Trim(),
                LineName = x.LineName.Trim(),
                ProductName = x.ProductName.Trim(),

                Quantity = x.Quantity,
                SalesAmount = x.SalesAmount,

                ImportedAt = DateTime.Now,
                UploadBatchId = uploadBatchId
            }).ToList();

            _context.SalesAnalyticsDailySalesReports.AddRange(entities);
            _context.SaveChanges();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void ReplaceDailyVisitsReport(List<SalesAnalyticsDailyVisitImportDto> visitRows,int uploadBatchId,DateTime reportDate)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var oldRows = _context.SalesAnalyticsDailyVisitReports
                .Where(x => x.ReportDate.Date == reportDate.Date)
                .ToList();

            if (oldRows.Any())
            {
                _context.SalesAnalyticsDailyVisitReports.RemoveRange(oldRows);
                _context.SaveChanges();
            }

            var entities = visitRows.Select(x => new SalesAnalyticsDailyVisitReportEntity
            {
                ReportDate = reportDate.Date,

                SupervisorName = x.SupervisorName?.Trim(),
                CityName = x.CityName?.Trim(),
                VisitCode = x.VisitCode?.Trim(),

                SalesRepCode = x.SalesRepCode.Trim(),
                SalesRepName = x.SalesRepName.Trim(),

                CustomerCode = x.CustomerCode.Trim(),
                CustomerName = x.CustomerName.Trim(),

                VisitStatus = x.VisitStatus?.Trim(),
                NegativeReason = x.NegativeReason?.Trim(),

                SuccessfulVisitValue = x.SuccessfulVisitValue,

                VisitStartTime = x.VisitStartTime,
                VisitEndTime = x.VisitEndTime,
                VisitDurationText = x.VisitDurationText?.Trim(),

                ImportedAt = DateTime.Now,
                UploadBatchId = uploadBatchId
            }).ToList();

            _context.SalesAnalyticsDailyVisitReports.AddRange(entities);
            _context.SaveChanges();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<SalesAnalyticsUploadHistoryDto> GetUploadHistory(int take = 50)
    {
        return _context.SalesAnalyticsUploadBatches
            .AsNoTracking()
            .OrderByDescending(x => x.UploadDate)
            .Take(take)
            .Select(x => new SalesAnalyticsUploadHistoryDto
            {
                Id = x.Id,
                FileType = x.FileType,
                Status = x.Status,
                OriginalFileName = x.OriginalFileName,
                UploadDate = x.UploadDate,
                ReportDate = x.ReportDate,
                TotalRows = x.TotalRows,
                ImportedRows = x.ImportedRows,
                FailedRows = x.FailedRows,
                UploadedBy = x.UploadedBy,
                ErrorMessage = x.ErrorMessage
            })
            .ToList();
    }

    public void ReplaceMtdSalesReport(
    List<SalesAnalyticsMtdSalesImportDto> salesRows,
    int uploadBatchId,
    DateTime toDate)
    {
        var normalizedToDate = toDate.Date;
        var fromDate = new DateTime(normalizedToDate.Year, normalizedToDate.Month, 1);

        var oldRows = _context.SalesAnalyticsMtdSalesReports
            .Where(x =>
                x.Year == normalizedToDate.Year &&
                x.Month == normalizedToDate.Month &&
                x.ToDate >= normalizedToDate &&
                x.ToDate < normalizedToDate.AddDays(1))
            .ToList();

        if (oldRows.Any())
        {
            _context.SalesAnalyticsMtdSalesReports.RemoveRange(oldRows);
        }

        var entities = salesRows.Select(x => new SalesAnalyticsMtdSalesReportEntity
        {
            Year = normalizedToDate.Year,
            Month = normalizedToDate.Month,
            FromDate = fromDate,
            ToDate = normalizedToDate,

            CityCode = x.CityCode,
            CityName = x.CityName,

            CustomerCode = x.CustomerCode,
            CustomerName = x.CustomerName,

            ProductCode = x.ProductCode,
            ProductName = x.ProductName,
            Unit = x.Unit,

            Quantity = x.Quantity,
            SalesAmount = x.SalesAmount,
            DiscountAmount = x.DiscountAmount,
            TaxPercentage = x.TaxPercentage,
            TaxAmount = x.TaxAmount,
            TotalBeforeTax = x.TotalBeforeTax,
            TotalAfterTax = x.TotalAfterTax,

            UploadBatchId = uploadBatchId,
            ImportedAt = DateTime.Now
        }).ToList();

        _context.SalesAnalyticsMtdSalesReports.AddRange(entities);
        _context.SaveChanges();
    }
}