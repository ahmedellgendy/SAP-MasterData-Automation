using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Enums;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Persistence;

public class SalesAnalyticsMasterDataRepository : ISalesAnalyticsMasterDataRepository
{
    private readonly ApplicationDbContext _context;

    public SalesAnalyticsMasterDataRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public int CreateUploadBatch(
        SalesAnalyticsUploadFileType fileType,
        string originalFileName,
        DateTime? reportDate,
        string? uploadedBy)
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

    public void CompleteUploadBatch(
        int uploadBatchId,
        SalesAnalyticsUploadStatus status,
        int totalRows,
        int importedRows,
        int failedRows,
        string? errorMessage = null)
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
}