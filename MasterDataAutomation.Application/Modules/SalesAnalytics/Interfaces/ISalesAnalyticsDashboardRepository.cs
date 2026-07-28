using MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;

public interface ISalesAnalyticsDashboardRepository
{
    SalesAnalyticsDashboardDto GetDashboard(DateTime reportDate);
    DateTime? GetLatestReportDate();

}