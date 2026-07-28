using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.SalesAnalytics;

[Authorize(Roles = "Admin,Manager")]
public class SalesAnalyticsDashboardController : Controller
{
    private readonly ISalesAnalyticsDashboardRepository _dashboardRepository;

    public SalesAnalyticsDashboardController(
        ISalesAnalyticsDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }



    [HttpGet]
    public IActionResult Index(DateTime? reportDate, string? branchCode)
    {
        var latestReportDate = _dashboardRepository.GetLatestReportDate();

        var selectedDate = reportDate?.Date
            ?? latestReportDate
            ?? DateTime.Today;

        var dashboard = _dashboardRepository.GetDashboard(selectedDate, branchCode);

        ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
        ViewBag.SelectedBranchCode = branchCode ?? string.Empty;
        ViewBag.Branches = _dashboardRepository.GetBranchOptions();

        return View(dashboard);
    }
}