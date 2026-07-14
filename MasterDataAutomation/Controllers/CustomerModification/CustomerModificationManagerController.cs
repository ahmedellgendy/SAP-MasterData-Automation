using MasterDataAutomation.Application.Modules.CustomerModification.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using MasterDataAutomation.Application.Modules.CustomerModification.Enums;


namespace MasterDataAutomation.Web.Controllers.CustomerModification;

[Authorize(Roles = "Admin,Manager")]
public class CustomerModificationManagerController : Controller
{
    private readonly ICustomerModificationRequestRepository _repository;

    public CustomerModificationManagerController(ICustomerModificationRequestRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var requests = _repository.GetSubmitted();

        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Approve(int id)
    {
        var result = _repository.Approve(id);

        if (!result)
        {
            TempData["Error"] = "لم يتم العثور على الطلب أو تم مراجعته بالفعل.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "تمت الموافقة على طلب تعديل العميل بنجاح.";

        return RedirectToAction(nameof(Index));
    }

  

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Reject(int id, string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            TempData["Error"] = "سبب الرفض مطلوب.";
            return RedirectToAction(nameof(Index));
        }

        var result = _repository.Reject(id, rejectionReason);

        if (!result)
        {
            TempData["Error"] = "لم يتم العثور على الطلب أو تم مراجعته بالفعل.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "تم رفض طلب تعديل العميل.";

        return RedirectToAction(nameof(Index));
    }

  

    [HttpGet]
    public IActionResult Approved()
    {
        var requests = _repository.GetApproved();

        return View(requests);
    }

    [HttpGet]
    public IActionResult ExportApproved()
    {
        var requests = _repository.GetApproved();

        var exportedIds = requests.Select(x => x.Id).ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Customer Modifications");

        worksheet.Style.Font.FontName = "Segoe UI";
        worksheet.Style.Font.FontSize = 11;

        // Title
        worksheet.Range("A1:G1").Merge();
        worksheet.Cell("A1").Value = "Approved Customer Modification Requests";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        worksheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9EAD3");
        worksheet.Row(1).Height = 28;

        // Export date
        worksheet.Range("A2:G2").Merge();
        worksheet.Cell("A2").Value = $"Export Date: {DateTime.Now:yyyy-MM-dd HH:mm}";
        worksheet.Cell("A2").Style.Font.Italic = true;
        worksheet.Cell("A2").Style.Font.FontColor = XLColor.Gray;
        worksheet.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Headers
        var headerRow = 4;

        worksheet.Cell(headerRow, 1).Value = "Branch";
        worksheet.Cell(headerRow, 2).Value = "Requested By";
        worksheet.Cell(headerRow, 3).Value = "Customer Code";
        worksheet.Cell(headerRow, 4).Value = "Customer Name";
        worksheet.Cell(headerRow, 5).Value = "Modification Type";
        worksheet.Cell(headerRow, 6).Value = "New Customer Name";
        worksheet.Cell(headerRow, 7).Value = "Notes";

        var headerRange = worksheet.Range(headerRow, 1, headerRow, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#2F7D6B");
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        worksheet.Row(headerRow).Height = 24;

        var row = headerRow + 1;

        foreach (var item in requests)
        {
            worksheet.Cell(row, 1).Value = item.BranchName;
            worksheet.Cell(row, 2).Value = item.CreatedBy ?? "";
            worksheet.Cell(row, 3).Value = item.MarketCode;
            worksheet.Cell(row, 4).Value = item.MarketName;
            worksheet.Cell(row, 5).Value = GetModificationTypeText(item.ModificationType);

            worksheet.Cell(row, 6).Value =
                item.ModificationType == CustomerModificationType.ChangeName
                    ? item.NewMarketName ?? ""
                    : "";

            worksheet.Cell(row, 7).Value = item.Notes ?? "";

            // row styling
            var dataRange = worksheet.Range(row, 1, row, 7);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // zebra row color
            if (row % 2 == 0)
            {
                dataRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
            }

            row++;
        }

        var lastRow = row - 1;

        if (lastRow >= headerRow)
        {
            var fullRange = worksheet.Range(headerRow, 1, Math.Max(lastRow, headerRow), 7);
            fullRange.SetAutoFilter();
        }

        // Important formatting
        worksheet.SheetView.FreezeRows(headerRow);

        worksheet.Column(1).Width = 18; // Branch
        worksheet.Column(2).Width = 22; // Requested By
        worksheet.Column(3).Width = 18; // Customer Code
        worksheet.Column(4).Width = 35; // Customer Name
        worksheet.Column(5).Width = 24; // Modification Type
        worksheet.Column(6).Width = 35; // New Customer Name
        worksheet.Column(7).Width = 45; // Notes

        // Customer Code as text
        worksheet.Column(3).Style.NumberFormat.Format = "@";
        worksheet.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        // Text alignment
        worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Branch
        worksheet.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;   // Requested By
        worksheet.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;   // Customer Code
        worksheet.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;   // Customer Name
        worksheet.Column(5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Modification Type
        worksheet.Column(6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;   // New Customer Name
        worksheet.Column(7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;   // Notes

        // Wrap notes
        worksheet.Column(7).Style.Alignment.WrapText = true;

        // Make used range neat
        var usedRange = worksheet.RangeUsed();

        if (usedRange != null)
        {
            usedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var fileBytes = stream.ToArray();

        _repository.DeleteApprovedByIds(exportedIds);

        var fileName = $"Approved_Customer_Modifications_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    private static string GetModificationTypeText(CustomerModificationType type)
    {
        return type switch
        {
            CustomerModificationType.RemoveFridge => "رفع الثلاجة",
            CustomerModificationType.ChangeName => "تعديل اسم",
            CustomerModificationType.ChangeToPrivate => "تحويل إلى خاص",
            CustomerModificationType.ChangeToFridge => "تحويل إلى ثلاجة",
            _ => "-"
        };
    }

}