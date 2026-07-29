using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;

public interface ISalesAnalyticsMasterDataImportService
{
    SalesAnalyticsImportResultDto ImportCustomersMaster(
        Stream fileStream,
        string originalFileName,
        string? uploadedBy);

    SalesAnalyticsImportResultDto ImportSalesRepsMaster(
        Stream fileStream,
        string originalFileName,
        string? uploadedBy);

    SalesAnalyticsImportResultDto ImportDailySalesReport(
    Stream fileStream,
    string originalFileName,
    DateTime reportDate,
    string? uploadedBy);

    SalesAnalyticsImportResultDto ImportMtdSalesReport(
    Stream fileStream,
    string originalFileName,
    DateTime toDate,
    string? uploadedBy);

    SalesAnalyticsImportResultDto ImportDailyVisitsReport(
    Stream fileStream,
    string originalFileName,
    DateTime reportDate,
    string? uploadedBy);

    SalesAnalyticsImportResultDto ImportMonthlyTargets(
    Stream fileStream,
    string originalFileName,
    string? uploadedBy);
}