using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.SalesAnalytics;

[Authorize(Roles = "Admin,Manager")]
public class SalesAnalyticsMasterDataController : Controller
{
    private readonly ISalesAnalyticsMasterDataImportService _importService;

    public SalesAnalyticsMasterDataController(ISalesAnalyticsMasterDataImportService importService)
    {
        _importService = importService;
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

}