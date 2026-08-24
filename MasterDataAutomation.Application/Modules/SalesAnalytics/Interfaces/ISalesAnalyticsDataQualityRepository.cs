using MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;

public interface ISalesAnalyticsDataQualityRepository
{
    SalesAnalyticsDataQualityReportDto GetReport(DateTime reportDate, string? branchCode = null);
}