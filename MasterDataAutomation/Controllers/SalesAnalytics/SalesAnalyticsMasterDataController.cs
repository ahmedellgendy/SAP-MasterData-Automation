using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.SalesAnalytics;

[Authorize(Roles = "Admin,Manager")]
public class SalesAnalyticsMasterDataController : Controller
{
    private readonly ISalesAnalyticsMasterDataImportService _importService;
    private readonly ISalesAnalyticsMasterDataRepository _repository;

    public SalesAnalyticsMasterDataController(ISalesAnalyticsMasterDataImportService importService, ISalesAnalyticsMasterDataRepository repository)

    {
        _importService = importService;
        _repository = repository;
    }

    [HttpGet]
    public IActionResult CustomerMaster()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UploadCustomerMaster(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid Excel file.";
            return RedirectToAction(nameof(CustomerMaster));
        }

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "Only Excel files are allowed.";
            return RedirectToAction(nameof(CustomerMaster));
        }

        var uploadedBy = User.Identity?.Name ?? "Unknown";

        using var stream = file.OpenReadStream();

        var result = _importService.ImportCustomersMaster(
            stream,
            file.FileName,
            uploadedBy);

        if (!result.Success)
        {
            TempData["Error"] = result.Message ?? "Import failed.";
            return RedirectToAction(nameof(CustomerMaster));
        }

        TempData["Success"] =
            $"Customer Master imported successfully. Imported: {result.ImportedRows}, Failed: {result.FailedRows}";

        return RedirectToAction(nameof(CustomerMaster));
    }

    [HttpGet]
    public IActionResult SalesRepMaster()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UploadSalesRepMaster(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid Excel file.";
            return RedirectToAction(nameof(SalesRepMaster));
        }

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "Only Excel files are allowed.";
            return RedirectToAction(nameof(SalesRepMaster));
        }

        var uploadedBy = User.Identity?.Name ?? "Unknown";

        using var stream = file.OpenReadStream();

        var result = _importService.ImportSalesRepsMaster(
            stream,
            file.FileName,
            uploadedBy);

        if (!result.Success)
        {
            TempData["Error"] = result.Message ?? "Import failed.";
            return RedirectToAction(nameof(SalesRepMaster));
        }

        TempData["Success"] =
            $"Sales Rep Master imported successfully. Imported: {result.ImportedRows}, Failed: {result.FailedRows}";

        return RedirectToAction(nameof(SalesRepMaster));
    }

    [HttpGet]
    public IActionResult DailySalesReport()
    {
        ViewBag.DefaultDate = DateTime.Today.ToString("yyyy-MM-dd");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UploadDailySalesReport(IFormFile file, DateTime reportDate)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid Excel file.";
            return RedirectToAction(nameof(DailySalesReport));
        }

        if (reportDate == default)
        {
            TempData["Error"] = "Please select report date.";
            return RedirectToAction(nameof(DailySalesReport));
        }

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "Only Excel files are allowed.";
            return RedirectToAction(nameof(DailySalesReport));
        }

        var uploadedBy = User.Identity?.Name ?? "Unknown";

        using var stream = file.OpenReadStream();

        var result = _importService.ImportDailySalesReport(
            stream,
            file.FileName,
            reportDate,
            uploadedBy);

        if (!result.Success)
        {
            TempData["Error"] = result.Message ?? "Import failed.";
            return RedirectToAction(nameof(DailySalesReport));
        }

        TempData["Success"] =
            $"Daily sales report imported successfully. Imported: {result.ImportedRows}, Failed: {result.FailedRows}";

        return RedirectToAction(nameof(DailySalesReport));
    }

    [HttpGet]
    public IActionResult DailyVisitsReport()
    {
        ViewBag.DefaultDate = DateTime.Today.ToString("yyyy-MM-dd");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UploadDailyVisitsReport(IFormFile file, DateTime reportDate)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid Excel file.";
            return RedirectToAction(nameof(DailyVisitsReport));
        }

        if (reportDate == default)
        {
            TempData["Error"] = "Please select report date.";
            return RedirectToAction(nameof(DailyVisitsReport));
        }

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "Only Excel files are allowed.";
            return RedirectToAction(nameof(DailyVisitsReport));
        }

        var uploadedBy = User.Identity?.Name ?? "Unknown";

        using var stream = file.OpenReadStream();

        var result = _importService.ImportDailyVisitsReport(
            stream,
            file.FileName,
            reportDate,
            uploadedBy);

        if (!result.Success)
        {
            TempData["Error"] = result.Message ?? "Import failed.";
            return RedirectToAction(nameof(DailyVisitsReport));
        }

        TempData["Success"] =
            $"Daily visits report imported successfully. Imported: {result.ImportedRows}, Failed: {result.FailedRows}";

        return RedirectToAction(nameof(DailyVisitsReport));
    }

    [HttpGet]
    public IActionResult UploadCenter()
    {
        var history = _repository.GetUploadHistory(50);
        return View(history);
    }

    [HttpGet]
    public IActionResult MtdSalesReport()
    {
        ViewBag.Today = DateTime.Today.ToString("yyyy-MM-dd");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(200_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 200_000_000)]
    public IActionResult UploadMtdSalesReport(IFormFile file, DateTime toDate)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select MTD sales report file.";
            return RedirectToAction(nameof(MtdSalesReport));
        }

        if (toDate == default)
        {
            TempData["Error"] = "Please select report To Date.";
            return RedirectToAction(nameof(MtdSalesReport));
        }

        using var stream = file.OpenReadStream();

        var result = _importService.ImportMtdSalesReport(
            stream,
            file.FileName,
            toDate,
            User.Identity?.Name);

        if (result.Success)
        {
            TempData["Success"] =
                $"MTD Sales Report uploaded successfully. Imported: {result.ImportedRows}, Failed: {result.FailedRows}";
        }
        else
        {
            TempData["Error"] = result.Message ?? "Failed to upload MTD Sales Report.";
        }

        return RedirectToAction(nameof(MtdSalesReport));
    }

}