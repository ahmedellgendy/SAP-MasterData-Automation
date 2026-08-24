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
    public IActionResult UploadMtdSalesReport(
    IFormFile file,
    DateTime fromDate,
    DateTime toDate)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] =
                "Please select Sales Range report file.";

            return RedirectToAction(
                nameof(MtdSalesReport));
        }

        if (fromDate == default)
        {
            TempData["Error"] =
                "Please select report From Date.";

            return RedirectToAction(
                nameof(MtdSalesReport));
        }

        if (toDate == default)
        {
            TempData["Error"] =
                "Please select report To Date.";

            return RedirectToAction(
                nameof(MtdSalesReport));
        }

        if (fromDate.Date > toDate.Date)
        {
            TempData["Error"] =
                "From Date cannot be after To Date.";

            return RedirectToAction(
                nameof(MtdSalesReport));
        }

        // مهم:
        // نخلي الـRange داخل نفس الشهر حاليًا
        // لأن حسابات الـDashboard والTargets شهرية.
        if (
            fromDate.Year != toDate.Year ||
            fromDate.Month != toDate.Month)
        {
            TempData["Error"] =
                "Sales Range must be within the same month.";

            return RedirectToAction(
                nameof(MtdSalesReport));
        }

        var extension =
            Path.GetExtension(
                file.FileName)
            .ToLowerInvariant();

        if (
            extension != ".xlsx" &&
            extension != ".xls")
        {
            TempData["Error"] =
                "Only Excel files are allowed.";

            return RedirectToAction(
                nameof(MtdSalesReport));
        }

        using var stream =
            file.OpenReadStream();

        var result =
            _importService.ImportMtdSalesReport(
                stream,
                file.FileName,
                fromDate.Date,
                toDate.Date,
                User.Identity?.Name);

        if (result.Success)
        {
            TempData["Success"] =
                $"Sales Range uploaded successfully. " +
                $"Period: {fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}. " +
                $"Imported: {result.ImportedRows}, " +
                $"Failed: {result.FailedRows}";
        }
        else
        {
            TempData["Error"] =
                result.Message ??
                "Failed to upload Sales Range report.";
        }

        return RedirectToAction(
            nameof(MtdSalesReport));
    }


    [HttpGet]
    public IActionResult MtdVisitsReport()
    {
        ViewBag.Today =
            DateTime.Today.ToString("yyyy-MM-dd");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(200_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 200_000_000)]
    public IActionResult UploadMtdVisitsReport(
    IFormFile file,
    DateTime fromDate,
    DateTime toDate)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] =
                "Please select Visits Range report file.";

            return RedirectToAction(
                nameof(MtdVisitsReport));
        }

        if (fromDate == default)
        {
            TempData["Error"] =
                "Please select report From Date.";

            return RedirectToAction(
                nameof(MtdVisitsReport));
        }

        if (toDate == default)
        {
            TempData["Error"] =
                "Please select report To Date.";

            return RedirectToAction(
                nameof(MtdVisitsReport));
        }

        if (fromDate.Date > toDate.Date)
        {
            TempData["Error"] =
                "From Date cannot be after To Date.";

            return RedirectToAction(
                nameof(MtdVisitsReport));
        }

        if (
            fromDate.Year != toDate.Year ||
            fromDate.Month != toDate.Month)
        {
            TempData["Error"] =
                "Visits Range must be within the same month.";

            return RedirectToAction(
                nameof(MtdVisitsReport));
        }

        var extension =
            Path.GetExtension(
                file.FileName)
            .ToLowerInvariant();

        if (
            extension != ".xlsx" &&
            extension != ".xls")
        {
            TempData["Error"] =
                "Only Excel files are allowed.";

            return RedirectToAction(
                nameof(MtdVisitsReport));
        }

        using var stream =
            file.OpenReadStream();

        var result =
            _importService.ImportMtdVisitsReport(
                stream,
                file.FileName,
                fromDate.Date,
                toDate.Date,
                User.Identity?.Name);

        if (result.Success)
        {
            TempData["Success"] =
                $"Visits Range uploaded successfully. " +
                $"Period: {fromDate:dd/MM/yyyy} - {toDate:dd/MM/yyyy}. " +
                $"Imported: {result.ImportedRows}, " +
                $"Failed: {result.FailedRows}";
        }
        else
        {
            TempData["Error"] =
                result.Message ??
                "Failed to upload Visits Range report.";
        }

        return RedirectToAction(
            nameof(MtdVisitsReport));
    }
}