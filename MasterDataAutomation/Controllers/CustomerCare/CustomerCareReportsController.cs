
using MasterDataAutomation.Application.Modules.CustomerCare.Dtos.Reports;
using MasterDataAutomation.Application.Modules.CustomerCare.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;

namespace MasterDataAutomation.Web.Controllers.CustomerCare;

[Authorize(Roles = "Admin,ComplaintManager")]
[Route("CustomerCare/Reports")]
public class CustomerCareReportsController : Controller
{
    private readonly ICustomerCareReportService _reportService;

    public CustomerCareReportsController(
        ICustomerCareReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(
        [FromQuery] CustomerCareReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetReportAsync(
            filter,
            cancellationToken);

        return View(report);
    }



    [HttpGet("ExportExcel")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] CustomerCareReportFilterDto filter,
        CancellationToken cancellationToken)
    {
        var report = await _reportService.GetReportAsync(
            filter, cancellationToken);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("تقرير خدمة العملاء");

        // Arabic RTL layout
        ws.RightToLeft = true;
        ws.ShowGridLines = false;

        var navy = XLColor.FromHtml("#142B49");
        var blue = XLColor.FromHtml("#176BBD");
        var light = XLColor.FromHtml("#EEF3F8");
        var white = XLColor.White;
        var green = XLColor.FromHtml("#16805D");
        var red = XLColor.FromHtml("#C0392B");
        var gray = XLColor.FromHtml("#64748B");

        ws.Style.Font.FontName = "Arial";
        ws.Style.Font.FontSize = 9;
        ws.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Right;
        ws.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        for (int col = 1; col <= 10; col++)
            ws.Column(col).Width = 13;

        // Main header
        var title = ws.Range(1, 1, 2, 10);
        title.Merge();
        title.Value = "FridayOps | تقرير أداء خدمة العملاء";
        title.Style.Fill.BackgroundColor = navy;
        title.Style.Font.FontColor = white;
        title.Style.Font.Bold = true;
        title.Style.Font.FontSize = 18;
        title.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        ws.Row(1).Height = 22;
        ws.Row(2).Height = 18;

        MergeText(ws, 3, 1, 10,
            "شركة فرايداي آيس كريم | التقرير التنفيذي على مستوى الشركة",
            gray);

        var from = filter.FromDate?.ToString("yyyy/MM/dd")
            ?? "بداية البيانات";

        var to = filter.ToDate?.ToString("yyyy/MM/dd")
            ?? "حتى الآن";

        MergeText(ws, 4, 1, 10,
            $"الفترة: {from} - {to}     |     تاريخ الإصدار: {DateTime.Now:yyyy/MM/dd HH:mm}",
            gray);

        // KPIs
        Section(ws, 6, "أولاً: المؤشرات الرئيسية", navy);

        var s = report.Summary;

        Metric(ws, 7, 1, "إجمالي الشكاوى", s.TotalTickets.ToString(), blue);
        Metric(ws, 7, 3, "المفتوحة", s.OpenTickets.ToString(), blue);
        Metric(ws, 7, 5, "المكتملة", s.CompletedTickets.ToString(), green);
        Metric(ws, 7, 7, "المتأخرة", s.OverdueTickets.ToString(), red);
        Metric(ws, 7, 9, "نسبة الإنجاز", $"{s.ResolutionRate:N1}%", green);

        Metric(ws, 10, 1, "الشكاوى الحرجة", s.CriticalTickets.ToString(), red);
        Metric(ws, 10, 3, "أعيد فتحها", s.ReopenedTickets.ToString(), navy);
        Metric(ws, 10, 5, "متوسط الحل", $"{s.AverageResolutionHours:N1} ساعة", navy);
        Metric(ws, 10, 7, "SLA الاستجابة", $"{s.FirstResponseComplianceRate:N1}%", blue);
        Metric(ws, 10, 9, "SLA الحل", $"{s.ResolutionComplianceRate:N1}%", blue);

        Section(ws, 14, "ثانياً: تحليل الشكاوى", navy);

        // Categories and Departments
        GroupHeader(ws, 15, 1, "حسب التصنيف");
        GroupHeader(ws, 15, 6, "حسب القسم المسؤول");

        WriteGroup(ws, 16, 1, report.ByCategory);
        WriteGroup(ws, 16, 6, report.ByDepartment);

        // Statuses and Priorities
        GroupHeader(ws, 24, 1, "حسب الحالة");
        GroupHeader(ws, 24, 6, "حسب الأولوية");

        WriteGroup(ws, 25, 1, report.ByStatus);
        WriteGroup(ws, 25, 6, report.ByPriority);

        // Sources
        GroupHeader(ws, 33, 1, "حسب مصدر الشكوى");
        WriteGroup(ws, 34, 1, report.BySource);

        // SLA summary
        GroupHeader(ws, 33, 6, "مؤشرات المتابعة");

        MergeText(ws, 34, 6, 10,
            $"تجاوزات وقت الاستجابة: {s.FirstResponseBreachedTickets}",
            red);

        MergeText(ws, 36, 6, 10,
            $"تجاوزات وقت الحل: {s.ResolutionBreachedTickets}",
            red);

        MergeText(ws, 38, 6, 10,
            $"الشكاوى المغلقة: {s.ClosedTickets}",
            navy);

        // Footer
        var footer = ws.Range(43, 1, 43, 10);
        footer.Merge();
        footer.Value = "FridayOps | Developed by Ahmed Elgendy";
        footer.Style.Font.FontSize = 8;
        footer.Style.Font.FontColor = gray;
        footer.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        ws.Row(43).Height = 20;

        // Print settings
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
        ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
        ws.PageSetup.FitToPages(1, 1);
        ws.PageSetup.CenterHorizontally = true;
        ws.PageSetup.CenterVertically = true;
        ws.PageSetup.Margins.Top = 0.25;
        ws.PageSetup.Margins.Bottom = 0.25;
        ws.PageSetup.Margins.Left = 0.25;
        ws.PageSetup.Margins.Right = 0.25;
        ws.PageSetup.PrintAreas.Add("A1:J43");

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"FridayOps_CustomerCare_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }

    private static void Section(
        IXLWorksheet ws, int row, string title, XLColor color)
    {
        var range = ws.Range(row, 1, row, 10);
        range.Merge();
        range.Value = title;
        range.Style.Fill.BackgroundColor = color;
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.Bold = true;
        range.Style.Font.FontSize = 11;
        ws.Row(row).Height = 23;
    }

    private static void MergeText(
        IXLWorksheet ws, int row,
        int fromCol, int toCol,
        string value, XLColor color)
    {
        var range = ws.Range(row, fromCol, row, toCol);
        range.Merge();
        range.Value = value;
        range.Style.Font.FontColor = color;
        range.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;
        ws.Row(row).Height = 19;
    }

    private static void Metric(
        IXLWorksheet ws, int row, int col,
        string label, string value, XLColor color)
    {
        var labelRange = ws.Range(row, col, row, col + 1);
        labelRange.Merge();
        labelRange.Value = label;

        labelRange.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#EEF3F8");
        labelRange.Style.Font.FontColor =
            XLColor.FromHtml("#64748B");
        labelRange.Style.Font.FontSize = 8;
        labelRange.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        var valueRange = ws.Range(row + 1, col, row + 1, col + 1);
        valueRange.Merge();
        valueRange.Value = value;
        valueRange.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#EEF3F8");
        valueRange.Style.Font.Bold = true;
        valueRange.Style.Font.FontSize = 12;
        valueRange.Style.Font.FontColor = color;
        valueRange.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        ws.Row(row).Height = 21;
        ws.Row(row + 1).Height = 28;
    }

    private static void GroupHeader(
        IXLWorksheet ws, int row, int col, string title)
    {
        var range = ws.Range(row, col, row, col + 4);
        range.Merge();
        range.Value = title;
        range.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#DCE8F5");
        range.Style.Font.Bold = true;
        range.Style.Font.FontColor =
            XLColor.FromHtml("#142B49");
        ws.Row(row).Height = 22;
    }

    private static void WriteGroup(
        IXLWorksheet ws,
        int startRow,
        int startCol,
        IEnumerable<CustomerCareReportBreakdownDto> items)
    {
        var all = items
            .OrderByDescending(x => x.TotalTickets)
            .ToList();

        if (all.Count == 0)
        {
            MergeText(ws, startRow, startCol, startCol + 4,
                "لا توجد بيانات", XLColor.Gray);
            return;
        }

        // First five groups + aggregated remainder
        var top = all.Take(5).ToList();

        var display = top.Select(x => new
        {
            x.Name,
            x.TotalTickets,
            x.CompletedTickets
        }).ToList();

        if (all.Count > 5)
        {
            display.Add(new
            {
                Name = "باقي التصنيفات",
                TotalTickets = all.Skip(5).Sum(x => x.TotalTickets),
                CompletedTickets = all.Skip(5).Sum(x => x.CompletedTickets)
            });
        }

        int row = startRow;

        foreach (var item in display)
        {
            var name = ws.Range(
                row, startCol, row, startCol + 2);
            name.Merge();
            name.Value = item.Name;

            ws.Cell(row, startCol + 3).Value =
                item.TotalTickets;

            ws.Cell(row, startCol + 4).Value =
                item.TotalTickets == 0
                    ? 0
                    : Math.Round(
                        (decimal)item.CompletedTickets *
                        100m / item.TotalTickets, 1);

            ws.Cell(row, startCol + 4)
                .Style.NumberFormat.Format = "0.0\"%\"";

            if ((row - startRow) % 2 == 0)
            {
                ws.Range(row, startCol, row, startCol + 4)
                    .Style.Fill.BackgroundColor =
                        XLColor.FromHtml("#F6F8FB");
            }

            ws.Row(row).Height = 20;
            row++;
        }
    }


}
