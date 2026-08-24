using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.SalesAnalytics;

[Authorize(Roles = "Admin,Manager,CEO")]
public class SalesAnalyticsDataQualityController : Controller
{
    private readonly ISalesAnalyticsDataQualityRepository _dataQualityRepository;
    private readonly ISalesAnalyticsDashboardRepository _dashboardRepository;

    public SalesAnalyticsDataQualityController(
        ISalesAnalyticsDataQualityRepository dataQualityRepository,
        ISalesAnalyticsDashboardRepository dashboardRepository)
    {
        _dataQualityRepository = dataQualityRepository;
        _dashboardRepository = dashboardRepository;
    }

    [HttpGet]
    public IActionResult Index(DateTime? reportDate, string? branchCode)
    {
        var latestReportDate = _dashboardRepository.GetLatestReportDate();

        var selectedDate = reportDate?.Date
            ?? latestReportDate
            ?? DateTime.Today;

        var report = _dataQualityRepository.GetReport(selectedDate, branchCode);

        ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
        ViewBag.SelectedBranchCode = branchCode ?? string.Empty;
        ViewBag.Branches = _dashboardRepository.GetBranchOptions();

        return View(report);
    }
}