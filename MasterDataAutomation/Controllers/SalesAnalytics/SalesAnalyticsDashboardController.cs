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
    public IActionResult Index(DateTime? reportDate)
    {
        var latestReportDate = _dashboardRepository.GetLatestReportDate();

        var selectedDate = reportDate?.Date
            ?? latestReportDate
            ?? DateTime.Today;

        var dashboard = _dashboardRepository.GetDashboard(selectedDate);

        ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");

        return View(dashboard);
    }
}