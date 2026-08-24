using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Enums;

namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;

public interface ISalesAnalyticsMasterDataRepository
{
    int CreateUploadBatch(
        SalesAnalyticsUploadFileType fileType,
        string originalFileName,
        DateTime? reportDate,
        string? uploadedBy);

    void CompleteUploadBatch(
        int uploadBatchId,
        SalesAnalyticsUploadStatus status,
        int totalRows,
        int importedRows,
        int failedRows,
        string? errorMessage = null);

    void ReplaceCustomers(List<SalesAnalyticsCustomerImportDto> customers, int uploadBatchId);

    void ReplaceSalesReps(List<SalesAnalyticsSalesRepImportDto> salesReps, int uploadBatchId);

    void ReplaceRepRouteAssignments(List<SalesRepRouteAssignmentImportDto> assignments, int uploadBatchId);

    void ReplaceDailySalesReport(
    List<SalesAnalyticsDailySalesImportDto> salesRows,
    int uploadBatchId,
    DateTime reportDate);

    void ReplaceMtdSalesReport(
    List<SalesAnalyticsMtdSalesImportDto> salesRows,
    int uploadBatchId,
    DateTime fromDate,
    DateTime toDate);

    void ReplaceDailyVisitsReport(
    List<SalesAnalyticsDailyVisitImportDto> visitRows,
    int uploadBatchId,
    DateTime reportDate);

    void ReplaceMtdVisitsReport(
    List<SalesAnalyticsMtdVisitImportDto> visitRows,
    int uploadBatchId,
    DateTime fromDate,
    DateTime toDate);

    List<SalesAnalyticsUploadHistoryDto> GetUploadHistory(int take = 50);
}