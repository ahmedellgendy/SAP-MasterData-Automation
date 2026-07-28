using ClosedXML.Excel;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.SalesAnalytics;

[Authorize(Roles = "Admin,Manager")]
public class SalesAnalyticsTargetsController : Controller
{
    private readonly ISalesDistrictMonthlyTargetRepository _targetRepository;
    private readonly ISalesAnalyticsMasterDataImportService _importService;


    public SalesAnalyticsTargetsController(
        ISalesDistrictMonthlyTargetRepository targetRepository,
        ISalesAnalyticsMasterDataImportService importService)
    {
        _targetRepository = targetRepository;
        _importService = importService;
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UploadMonthlyTargets(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a valid Excel file.";
            return RedirectToAction(nameof(Index));
        }

        var extension = Path.GetExtension(file.FileName).ToLower();

        if (extension != ".xlsx" && extension != ".xls")
        {
            TempData["Error"] = "Only Excel files are allowed.";
            return RedirectToAction(nameof(Index));
        }

        var uploadedBy = User.Identity?.Name ?? "Unknown";

        using var stream = file.OpenReadStream();

        var result = _importService.ImportMonthlyTargets(
            stream,
            file.FileName,
            uploadedBy);

        if (!result.Success)
        {
            TempData["Error"] = result.Message ?? "Import failed.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] =
            $"Monthly targets imported successfully. Imported: {result.ImportedRows}, Failed: {result.FailedRows}";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult DownloadTemplate(int? year, int? month)
    {
        var selectedYear = year ?? DateTime.Today.Year;
        var selectedMonth = month ?? DateTime.Today.Month;

        var salesDistricts = _targetRepository.GetSalesDistrictOptions();

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Monthly Targets");

        worksheet.Cell(1, 1).Value = "Year";
        worksheet.Cell(1, 2).Value = "Month";
        worksheet.Cell(1, 3).Value = "SalesDistrictCode";
        worksheet.Cell(1, 4).Value = "SalesDistrictName";
        worksheet.Cell(1, 5).Value = "BranchName";
        worksheet.Cell(1, 6).Value = "MonthlySalesTarget";
        worksheet.Cell(1, 7).Value = "PlannedVisits";

        var headerRange = worksheet.Range(1, 1, 1, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0B2A5B");
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        var row = 2;

        foreach (var district in salesDistricts)
        {
            worksheet.Cell(row, 1).Value = selectedYear;
            worksheet.Cell(row, 2).Value = selectedMonth;
            worksheet.Cell(row, 3).Value = district.SalesDistrictCode;
            worksheet.Cell(row, 4).Value = district.SalesDistrictName;
            worksheet.Cell(row, 5).Value = district.BranchName ?? "";
            worksheet.Cell(row, 6).Value = 0;
            worksheet.Cell(row, 7).Value = 0;

            row++;
        }

        var lastRow = row - 1;

        if (lastRow >= 2)
        {
            var dataRange = worksheet.Range(2, 1, lastRow, 7);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        worksheet.Column(1).Width = 12;
        worksheet.Column(2).Width = 12;
        worksheet.Column(3).Width = 22;
        worksheet.Column(4).Width = 35;
        worksheet.Column(5).Width = 25;
        worksheet.Column(6).Width = 24;
        worksheet.Column(7).Width = 18;

        worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        worksheet.Column(6).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Column(7).Style.NumberFormat.Format = "0";

        worksheet.SheetView.FreezeRows(1);
        worksheet.Range(1, 1, Math.Max(lastRow, 1), 7).SetAutoFilter();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var fileBytes = stream.ToArray();

        var fileName = $"Monthly_Targets_Template_{selectedYear}_{selectedMonth}.xlsx";

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}