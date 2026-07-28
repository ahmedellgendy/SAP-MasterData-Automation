using MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;

public interface ISalesAnalyticsDashboardRepository
{
    SalesAnalyticsDashboardDto GetDashboard(DateTime reportDate, string? branchCode = null);

    DateTime? GetLatestReportDate();

    List<BranchOptionDto> GetBranchOptions();
}