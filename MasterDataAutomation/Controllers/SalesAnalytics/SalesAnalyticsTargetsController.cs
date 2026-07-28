using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.SalesAnalytics;

[Authorize(Roles = "Admin,Manager")]
public class SalesAnalyticsTargetsController : Controller
{
    private readonly ISalesDistrictMonthlyTargetRepository _targetRepository;

    public SalesAnalyticsTargetsController(
        ISalesDistrictMonthlyTargetRepository targetRepository)
    {
        _targetRepository = targetRepository;
    }

    [HttpGet]
    public IActionResult Index(int? year, int? month)
    {
        var selectedYear = year ?? DateTime.Today.Year;
        var selectedMonth = month ?? DateTime.Today.Month;

        var targets = _targetRepository.GetTargets(selectedYear, selectedMonth);

        ViewBag.Year = selectedYear;
        ViewBag.Month = selectedMonth;
        ViewBag.SalesDistricts = _targetRepository.GetSalesDistrictOptions();

        return View(targets);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save(SaveSalesDistrictMonthlyTargetDto model)
    {
        if (model.Year < 2000 || model.Year > 2100)
        {
            TempData["Error"] = "Invalid year.";
            return RedirectToAction(nameof(Index), new { year = model.Year, month = model.Month });
        }

        if (model.Month < 1 || model.Month > 12)
        {
            TempData["Error"] = "Invalid month.";
            return RedirectToAction(nameof(Index), new { year = model.Year, month = model.Month });
        }

        if (string.IsNullOrWhiteSpace(model.SalesDistrictCode))
        {
            TempData["Error"] = "Please select sales district.";
            return RedirectToAction(nameof(Index), new { year = model.Year, month = model.Month });
        }

        if (model.MonthlySalesTarget < 0)
        {
            TempData["Error"] = "Monthly sales target cannot be negative.";
            return RedirectToAction(nameof(Index), new { year = model.Year, month = model.Month });
        }

        if (model.PlannedVisits < 0)
        {
            TempData["Error"] = "Planned visits cannot be negative.";
            return RedirectToAction(nameof(Index), new { year = model.Year, month = model.Month });
        }

        var option = _targetRepository.GetSalesDistrictOptions()
            .FirstOrDefault(x => x.SalesDistrictCode == model.SalesDistrictCode);

        if (option == null)
        {
            TempData["Error"] = "Selected sales district not found in customer master.";
            return RedirectToAction(nameof(Index), new { year = model.Year, month = model.Month });
        }

        model.SalesDistrictName = option.SalesDistrictName;
        model.BranchCode = option.BranchCode;
        model.BranchName = option.BranchName;

        var userName = User.Identity?.Name ?? "Unknown";

        _targetRepository.SaveTarget(model, userName);

        TempData["Success"] = "Target saved successfully.";

        return RedirectToAction(nameof(Index), new { year = model.Year, month = model.Month });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, int year, int month)
    {
        var deleted = _targetRepository.Delete(id);

        TempData[deleted ? "Success" : "Error"] =
            deleted ? "Target deleted successfully." : "Target not found.";

        return RedirectToAction(nameof(Index), new { year, month });
    }
}