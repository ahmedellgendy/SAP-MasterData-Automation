using ClosedXML.Excel;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterDataAutomation.Web.Controllers.SalesAnalytics;

[Authorize(Roles = "Admin,Manager,CEO")]
public class SalesAnalyticsExecutiveReportsController : Controller
{
    private readonly ISalesAnalyticsDashboardRepository _dashboardRepository;

    public SalesAnalyticsExecutiveReportsController(
        ISalesAnalyticsDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    // =========================================================
    // Customers With No Sales
    // =========================================================

    [HttpGet]
    public IActionResult NoSalesCustomers(
        DateTime? reportDate,
        string? branchCode = null)
    {
        var selectedDate =
            reportDate?.Date
            ?? _dashboardRepository.GetLatestReportDate()?.Date
            ?? DateTime.Today;

        var customers =
            _dashboardRepository.GetNoSalesCustomers(
                selectedDate,
                branchCode);

        ViewBag.SelectedDate =
            selectedDate.ToString("yyyy-MM-dd");

        ViewBag.SelectedBranchCode =
            branchCode ?? string.Empty;

        ViewBag.Branches =
            _dashboardRepository.GetBranchOptions();

        ViewBag.TotalCustomers =
            customers.Count;

        ViewBag.CriticalCustomers =
            customers.Count(x =>
                x.RiskLevel == "حرج");

        ViewBag.UrgentCustomers =
            customers.Count(x =>
                x.RiskLevel == "يحتاج متابعة عاجلة");

        ViewBag.TotalVisits =
            customers.Sum(x =>
                x.TotalVisits);

        ViewBag.TotalNegativeVisits =
            customers.Sum(x =>
                x.NegativeVisits);

        return View(customers);
    }

    // =========================================================
    // Export Customers With No Sales
    // =========================================================

    [HttpGet]
    public IActionResult ExportNoSalesCustomersExcel(
        DateTime? reportDate,
        string? branchCode = null)
    {
        var selectedDate =
            reportDate?.Date
            ?? _dashboardRepository.GetLatestReportDate()?.Date
            ?? DateTime.Today;

        var customers =
            _dashboardRepository.GetNoSalesCustomers(
                selectedDate,
                branchCode);

        using var workbook =
            new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add(
                "No Sales Customers");

        // =====================================================
        // Report Header
        // =====================================================

        worksheet.Cell(1, 1).Value =
            "FridayOps - تقرير العملاء غير المسحوبين";

        worksheet.Range(1, 1, 1, 13)
            .Merge();

        worksheet.Cell(2, 1).Value =
            $"البيانات من بداية الشهر حتى {selectedDate:dd/MM/yyyy}";

        worksheet.Range(2, 1, 2, 13)
            .Merge();

        worksheet.Cell(3, 1).Value =
            string.IsNullOrWhiteSpace(branchCode)
                ? "كل الفروع"
                : $"الفرع: {branchCode}";

        worksheet.Range(3, 1, 3, 13)
            .Merge();

        // =====================================================
        // Columns
        // =====================================================

        var headers = new[]
        {
            "كود العميل",
            "اسم العميل",
            "كود الفرع",
            "اسم الفرع",
            "كود منطقة البيع",
            "اسم منطقة البيع",
            "كود المندوب",
            "اسم المندوب",
            "إجمالي الزيارات",
            "الزيارات الإيجابية",
            "الزيارات السلبية",
            "أكثر سبب سلبي",
            "مستوى المتابعة"
        };

        for (var column = 0;
             column < headers.Length;
             column++)
        {
            worksheet.Cell(
                    5,
                    column + 1)
                .Value =
                headers[column];
        }

        // =====================================================
        // Data
        // =====================================================

        var row = 6;

        foreach (var customer in customers)
        {
            worksheet.Cell(row, 1).Value =
                customer.CustomerCode;

            worksheet.Cell(row, 2).Value =
                customer.CustomerName;

            worksheet.Cell(row, 3).Value =
                customer.BranchCode;

            worksheet.Cell(row, 4).Value =
                customer.BranchName;

            worksheet.Cell(row, 5).Value =
                customer.SalesDistrictCode;

            worksheet.Cell(row, 6).Value =
                customer.SalesDistrictName;

            worksheet.Cell(row, 7).Value =
                customer.SalesRepCode;

            worksheet.Cell(row, 8).Value =
                customer.SalesRepName;

            worksheet.Cell(row, 9).Value =
                customer.TotalVisits;

            worksheet.Cell(row, 10).Value =
                customer.PositiveVisits;

            worksheet.Cell(row, 11).Value =
                customer.NegativeVisits;

            worksheet.Cell(row, 12).Value =
                customer.TopNegativeReason
                ?? "بدون سبب مسجل";

            worksheet.Cell(row, 13).Value =
                customer.RiskLevel;

            row++;
        }

        // =====================================================
        // Styling
        // =====================================================

        worksheet.RightToLeft = true;

        var titleRange =
            worksheet.Range(
                1,
                1,
                1,
                headers.Length);

        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;

        var headerRange =
            worksheet.Range(
                5,
                1,
                5,
                headers.Length);

        headerRange.Style.Font.Bold = true;

        headerRange.Style.Alignment
            .Horizontal =
            XLAlignmentHorizontalValues.Center;

        headerRange.Style.Alignment
            .Vertical =
            XLAlignmentVerticalValues.Center;

        if (customers.Count > 0)
        {
            var dataRange =
                worksheet.Range(
                    5,
                    1,
                    row - 1,
                    headers.Length);

            dataRange.CreateTable();
        }

        worksheet.Columns()
            .AdjustToContents();

        worksheet.SheetView
            .FreezeRows(5);

        // =====================================================
        // Download
        // =====================================================

        using var stream =
            new MemoryStream();

        workbook.SaveAs(stream);

        var fileName =
            $"FridayOps_NoSalesCustomers_{selectedDate:yyyy-MM-dd}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    
    // =========================================================
    // Sales Rep Field Performance
    // =========================================================

    [HttpGet]
    public IActionResult SalesRepEffectiveness(
        DateTime? reportDate,
        string? branchCode = null)
    {
        var selectedDate =
            reportDate?.Date
            ?? _dashboardRepository.GetLatestReportDate()?.Date
            ?? DateTime.Today;

        var reps =
            _dashboardRepository.GetSalesRepEffectiveness(
                selectedDate,
                branchCode);

        ViewBag.SelectedDate =
            selectedDate.ToString("yyyy-MM-dd");

        ViewBag.SelectedBranchCode =
            branchCode ?? string.Empty;

        ViewBag.Branches =
            _dashboardRepository.GetBranchOptions();

        ViewBag.TotalSalesReps =
            reps.Count;

        ViewBag.TotalVisitedCustomers =
            reps.Sum(x => x.VisitedCustomers);

        ViewBag.TotalVisits =
            reps.Sum(x => x.TotalVisits);

        ViewBag.TotalPositiveVisits =
            reps.Sum(x => x.PositiveVisits);

        ViewBag.TotalNegativeVisits =
            reps.Sum(x => x.NegativeVisits);

        var totalVisits =
            reps.Sum(x => x.TotalVisits);

        var totalPositiveVisits =
            reps.Sum(x => x.PositiveVisits);

        ViewBag.OverallPositiveVisitRate =
            totalVisits == 0
                ? 0
                : Math.Round(
                    (decimal)totalPositiveVisits /
                    totalVisits *
                    100,
                    2);

        ViewBag.TotalVisitValue =
            reps.Sum(x => x.TotalVisitValue);

        return View(reps);
    }


    // =========================================================
    // Export Sales Rep Field Performance
    // =========================================================

    [HttpGet]
    public IActionResult ExportSalesRepEffectivenessExcel(
        DateTime? reportDate,
        string? branchCode = null)
    {
        var selectedDate =
            reportDate?.Date
            ?? _dashboardRepository.GetLatestReportDate()?.Date
            ?? DateTime.Today;

        var reps =
            _dashboardRepository.GetSalesRepEffectiveness(
                selectedDate,
                branchCode);

        using var workbook =
            new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add(
                "Sales Rep Performance");

        worksheet.RightToLeft = true;

        // =====================================================
        // Title
        // =====================================================

        worksheet.Cell(1, 1).Value =
            "FridayOps - تقرير الأداء الميداني للمناديب";

        worksheet.Range(1, 1, 1, 11)
            .Merge();

        worksheet.Cell(2, 1).Value =
            $"البيانات من بداية الشهر حتى {selectedDate:dd/MM/yyyy}";

        worksheet.Range(2, 1, 2, 11)
            .Merge();

        worksheet.Cell(3, 1).Value =
            string.IsNullOrWhiteSpace(branchCode)
                ? "كل الفروع"
                : $"الفرع: {branchCode}";

        worksheet.Range(3, 1, 3, 11)
            .Merge();

        // =====================================================
        // Headers
        // =====================================================

        var headers = new[]
        {
        "الترتيب",
        "كود المندوب",
        "اسم المندوب",
        "الفرع",
        "العملاء الذين تمت زيارتهم",
        "إجمالي الزيارات",
        "الزيارات الإيجابية",
        "الزيارات السلبية",
        "نسبة الزيارات الإيجابية",
        "متوسط الزيارات لكل عميل",
        "إجمالي قيمة الزيارات"
    };

        for (var column = 0;
             column < headers.Length;
             column++)
        {
            worksheet.Cell(
                    5,
                    column + 1)
                .Value =
                headers[column];
        }

        // =====================================================
        // Data
        // =====================================================

        var row = 6;

        foreach (var rep in reps)
        {
            worksheet.Cell(row, 1).Value =
                rep.Rank;

            worksheet.Cell(row, 2).Value =
                rep.SalesRepCode;

            worksheet.Cell(row, 3).Value =
                rep.SalesRepName;

            worksheet.Cell(row, 4).Value =
                rep.BranchName;

            worksheet.Cell(row, 5).Value =
                rep.VisitedCustomers;

            worksheet.Cell(row, 6).Value =
                rep.TotalVisits;

            worksheet.Cell(row, 7).Value =
                rep.PositiveVisits;

            worksheet.Cell(row, 8).Value =
                rep.NegativeVisits;

            worksheet.Cell(row, 9).Value =
                rep.PositiveVisitRate / 100;

            worksheet.Cell(row, 10).Value =
                rep.AverageVisitsPerCustomer;

            worksheet.Cell(row, 11).Value =
                rep.TotalVisitValue;

            row++;
        }

        // =====================================================
        // Styling
        // =====================================================

        var titleRange =
            worksheet.Range(
                1,
                1,
                1,
                headers.Length);

        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;

        var headerRange =
            worksheet.Range(
                5,
                1,
                5,
                headers.Length);

        headerRange.Style.Font.Bold = true;

        headerRange.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        headerRange.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        if (reps.Count > 0)
        {
            worksheet.Range(
                    5,
                    1,
                    row - 1,
                    headers.Length)
                .CreateTable();

            worksheet.Range(
                    6,
                    9,
                    row - 1,
                    9)
                .Style.NumberFormat.Format =
                "0.00%";

            worksheet.Range(
                    6,
                    11,
                    row - 1,
                    11)
                .Style.NumberFormat.Format =
                "#,##0.00";
        }

        worksheet.Columns()
            .AdjustToContents();

        worksheet.SheetView
            .FreezeRows(5);

        // =====================================================
        // Download
        // =====================================================

        using var stream =
            new MemoryStream();

        workbook.SaveAs(stream);

        var fileName =
            $"FridayOps_SalesRepFieldPerformance_{selectedDate:yyyy-MM-dd}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    // =========================================================
    // Branch Performance Analysis
    // =========================================================

    [HttpGet]
    public IActionResult BranchPerformance(
        DateTime? reportDate,
        string? branchCode = null)
    {
        var selectedDate =
            reportDate?.Date
            ?? _dashboardRepository.GetLatestReportDate()?.Date
            ?? DateTime.Today;

        var branches =
            _dashboardRepository.GetBranchPerformanceAnalysis(
                selectedDate,
                branchCode);

        ViewBag.SelectedDate =
            selectedDate.ToString("yyyy-MM-dd");

        ViewBag.SelectedBranchCode =
            branchCode ?? string.Empty;

        ViewBag.Branches =
            _dashboardRepository.GetBranchOptions();

        ViewBag.TotalBranches =
            branches.Count;

        ViewBag.TotalTarget =
            branches.Sum(x => x.MonthlyTarget);

        ViewBag.TotalActualSales =
            branches.Sum(x => x.ActualSales);

        ViewBag.TotalRemaining =
            branches.Sum(x => x.RemainingToTarget);

        ViewBag.TotalVisits =
            branches.Sum(x => x.ActualVisits);

        ViewBag.TotalPositiveVisits =
            branches.Sum(x => x.PositiveVisits);

        ViewBag.TotalActiveSalesReps =
            branches.Sum(x => x.ActiveSalesReps);

        var totalTarget =
            branches.Sum(x => x.MonthlyTarget);

        var totalActualSales =
            branches.Sum(x => x.ActualSales);

        ViewBag.OverallAchievement =
            totalTarget <= 0
                ? 0
                : Math.Round(
                    totalActualSales /
                    totalTarget *
                    100,
                    2);

        return View(branches);
    }


    // =========================================================
    // Export Branch Performance
    // =========================================================

    [HttpGet]
    public IActionResult ExportBranchPerformanceExcel(
        DateTime? reportDate,
        string? branchCode = null)
    {
        var selectedDate =
            reportDate?.Date
            ?? _dashboardRepository.GetLatestReportDate()?.Date
            ?? DateTime.Today;

        var branches =
            _dashboardRepository.GetBranchPerformanceAnalysis(
                selectedDate,
                branchCode);

        using var workbook =
            new XLWorkbook();

        var worksheet =
            workbook.Worksheets.Add(
                "Branch Performance");

        worksheet.RightToLeft = true;

        // =====================================================
        // Title
        // =====================================================

        worksheet.Cell(1, 1).Value =
            "FridayOps - تحليل أداء الفروع";

        worksheet.Range(1, 1, 1, 18)
            .Merge();

        worksheet.Cell(2, 1).Value =
            $"البيانات من بداية الشهر حتى {selectedDate:dd/MM/yyyy}";

        worksheet.Range(2, 1, 2, 18)
            .Merge();

        worksheet.Cell(3, 1).Value =
            string.IsNullOrWhiteSpace(branchCode)
                ? "كل الفروع"
                : $"الفرع: {branchCode}";

        worksheet.Range(3, 1, 3, 18)
            .Merge();

        // =====================================================
        // Headers
        // =====================================================

        var headers = new[]
        {
        "الترتيب",
        "كود الفرع",
        "اسم الفرع",
        "التارجت الشهري",
        "المبيعات الفعلية",
        "نسبة التحقيق",
        "النسبة المتوقعة",
        "فجوة الأداء",
        "المتبقي للتارجت",
        "المطلوب يوميًا",
        "الزيارات المخططة",
        "الزيارات الفعلية",
        "تحقيق الزيارات",
        "الزيارات الإيجابية",
        "الزيارات السلبية",
        "نسبة الزيارات الإيجابية",
        "المناديب النشطون",
        "حالة الأداء"
    };

        for (var column = 0;
             column < headers.Length;
             column++)
        {
            worksheet.Cell(
                    5,
                    column + 1)
                .Value =
                headers[column];
        }

        // =====================================================
        // Data
        // =====================================================

        var row = 6;

        foreach (var branch in branches)
        {
            worksheet.Cell(row, 1).Value =
                branch.Rank;

            worksheet.Cell(row, 2).Value =
                branch.BranchCode;

            worksheet.Cell(row, 3).Value =
                branch.BranchName;

            worksheet.Cell(row, 4).Value =
                branch.MonthlyTarget;

            worksheet.Cell(row, 5).Value =
                branch.ActualSales;

            worksheet.Cell(row, 6).Value =
                branch.AchievementPercentage / 100;

            worksheet.Cell(row, 7).Value =
                branch.ExpectedAchievementPercentage / 100;

            worksheet.Cell(row, 8).Value =
                branch.PaceGapPercentage / 100;

            worksheet.Cell(row, 9).Value =
                branch.RemainingToTarget;

            worksheet.Cell(row, 10).Value =
                branch.RequiredDailySales;

            worksheet.Cell(row, 11).Value =
                branch.PlannedVisits;

            worksheet.Cell(row, 12).Value =
                branch.ActualVisits;

            worksheet.Cell(row, 13).Value =
                branch.VisitAchievementPercentage / 100;

            worksheet.Cell(row, 14).Value =
                branch.PositiveVisits;

            worksheet.Cell(row, 15).Value =
                branch.NegativeVisits;

            worksheet.Cell(row, 16).Value =
                branch.PositiveVisitPercentage / 100;

            worksheet.Cell(row, 17).Value =
                branch.ActiveSalesReps;

            worksheet.Cell(row, 18).Value =
                branch.PerformanceStatus;

            row++;
        }

        // =====================================================
        // Styling
        // =====================================================

        var titleRange =
            worksheet.Range(
                1,
                1,
                1,
                headers.Length);

        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;

        var headerRange =
            worksheet.Range(
                5,
                1,
                5,
                headers.Length);

        headerRange.Style.Font.Bold = true;

        headerRange.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        headerRange.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        if (branches.Count > 0)
        {
            worksheet.Range(
                    5,
                    1,
                    row - 1,
                    headers.Length)
                .CreateTable();

            worksheet.Range(
                    6,
                    6,
                    row - 1,
                    8)
                .Style.NumberFormat.Format =
                "0.00%";

            worksheet.Range(
                    6,
                    13,
                    row - 1,
                    13)
                .Style.NumberFormat.Format =
                "0.00%";

            worksheet.Range(
                    6,
                    16,
                    row - 1,
                    16)
                .Style.NumberFormat.Format =
                "0.00%";

            worksheet.Range(
                    6,
                    4,
                    row - 1,
                    5)
                .Style.NumberFormat.Format =
                "#,##0.00";

            worksheet.Range(
                    6,
                    9,
                    row - 1,
                    10)
                .Style.NumberFormat.Format =
                "#,##0.00";
        }

        worksheet.Columns()
            .AdjustToContents();

        worksheet.SheetView
            .FreezeRows(5);

        // =====================================================
        // Download
        // =====================================================

        using var stream =
            new MemoryStream();

        workbook.SaveAs(stream);

        var fileName =
            $"FridayOps_BranchPerformance_{selectedDate:yyyy-MM-dd}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}