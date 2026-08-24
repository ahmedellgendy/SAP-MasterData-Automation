using MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Persistence;

public class SalesAnalyticsDashboardRepository : ISalesAnalyticsDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public SalesAnalyticsDashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public SalesAnalyticsDashboardDto GetDashboard(DateTime reportDate, string? branchCode = null)
    {
        var date = reportDate.Date;
        var nextDate = date.AddDays(1);

        var monthStart = new DateTime(date.Year, date.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        var remainingDays = Math.Max(daysInMonth - date.Day + 1, 1);

        var selectedBranchCode = NormalizeCode(branchCode);

        var monthSalesRows = _context.SalesAnalyticsDailySalesReports
            .AsNoTracking()
            .Where(x => x.ReportDate >= monthStart && x.ReportDate < nextDate)
            .ToList();

        var monthVisitRows = _context.SalesAnalyticsDailyVisitReports
            .AsNoTracking()
            .Where(x => x.ReportDate >= monthStart && x.ReportDate < nextDate)
            .ToList();

        // =========================================================
        // Sales Ranges
        // =========================================================

        var salesRangeGroups =
            _context.SalesAnalyticsMtdSalesReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.FromDate <= date &&
                    x.ToDate <= date)
                .GroupBy(x => new
                {
                    x.UploadBatchId,
                    x.FromDate,
                    x.ToDate
                })
                .Select(g => new
                {
                    g.Key.UploadBatchId,
                    FromDate = g.Key.FromDate.Date,
                    ToDate = g.Key.ToDate.Date
                })
                .OrderBy(x => x.FromDate)
                .ThenBy(x => x.ToDate)
                .ToList();

        var selectedSalesRanges =
            SelectNonOverlappingRanges(
                salesRangeGroups
                    .Select(x => new DateRangeSelection
                    {
                        UploadBatchId = x.UploadBatchId,
                        FromDate = x.FromDate,
                        ToDate = x.ToDate
                    })
                    .ToList());

        var selectedSalesBatchIds =
            selectedSalesRanges
                .Select(x => x.UploadBatchId)
                .ToHashSet();

        var mtdSalesRows =
            selectedSalesBatchIds.Count == 0
                ? new List<SalesAnalyticsMtdSalesReportEntity>()
                : _context.SalesAnalyticsMtdSalesReports
                    .AsNoTracking()
                    .Where(x =>
                        selectedSalesBatchIds.Contains(
                            x.UploadBatchId))
                    .ToList();

        // =========================================================
        // Daily Sales Outside Uploaded Ranges
        // =========================================================

        var dailySalesRowsAfterMtd =
            monthSalesRows
                .Where(x =>
                    !IsDateCoveredByRanges(
                        x.ReportDate.Date,
                        selectedSalesRanges))
                .ToList();

        // =========================================================
        // Visits Ranges
        // =========================================================

        var visitRangeGroups =
            _context.SalesAnalyticsMtdVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.FromDate <= date &&
                    x.ToDate <= date)
                .GroupBy(x => new
                {
                    x.UploadBatchId,
                    x.FromDate,
                    x.ToDate
                })
                .Select(g => new
                {
                    g.Key.UploadBatchId,
                    FromDate = g.Key.FromDate.Date,
                    ToDate = g.Key.ToDate.Date
                })
                .OrderBy(x => x.FromDate)
                .ThenBy(x => x.ToDate)
                .ToList();

        var selectedVisitRanges =
            SelectNonOverlappingRanges(
                visitRangeGroups
                    .Select(x => new DateRangeSelection
                    {
                        UploadBatchId = x.UploadBatchId,
                        FromDate = x.FromDate,
                        ToDate = x.ToDate
                    })
                    .ToList());

        var selectedVisitBatchIds =
            selectedVisitRanges
                .Select(x => x.UploadBatchId)
                .ToHashSet();

        var mtdVisitRows =
            selectedVisitBatchIds.Count == 0
                ? new List<SalesAnalyticsMtdVisitReportEntity>()
                : _context.SalesAnalyticsMtdVisitReports
                    .AsNoTracking()
                    .Where(x =>
                        selectedVisitBatchIds.Contains(
                            x.UploadBatchId))
                    .ToList();

        // =========================================================
        // Daily Visits Outside Uploaded Ranges
        // =========================================================

        var dailyVisitRowsAfterMtd =
            monthVisitRows
                .Where(x =>
                    !IsDateCoveredByRanges(
                        x.ReportDate.Date,
                        selectedVisitRanges))
                .ToList();

        var monthlyTargets = _context.SalesDistrictMonthlyTargets
            .AsNoTracking()
            .Where(x => x.Year == date.Year && x.Month == date.Month)
            .ToList();

        var customers = _context.SalesAnalyticsCustomers
            .AsNoTracking()
            .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCode))
            .Select(x => new
            {
                x.CustomerCode,
                x.SalesDistrictCode,
                x.BranchCode,
                x.BranchName
            })
            .ToList();

        var customerDistrictMap = customers
            .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCode))
            .GroupBy(x => NormalizeCode(x.CustomerCode))
            .ToDictionary(
                g => g.Key,
                g => NormalizeCode(g.First().SalesDistrictCode));

        var customerBranchMap = customers
            .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCode))
            .GroupBy(x => NormalizeCode(x.CustomerCode))
            .ToDictionary(
                g => g.Key,
                g => NormalizeCode(g.First().BranchCode));

        var lineBranchMap = customers
            .Where(x => !string.IsNullOrWhiteSpace(x.SalesDistrictCode))
            .GroupBy(x => NormalizeCode(x.SalesDistrictCode))
            .ToDictionary(
                g => g.Key,
                g => NormalizeCode(g.First().BranchCode));

        if (!string.IsNullOrWhiteSpace(selectedBranchCode))
        {
            monthSalesRows = monthSalesRows
                .Where(x => IsDailySalesInBranch(x, selectedBranchCode, customerBranchMap, lineBranchMap))
                .ToList();

            monthVisitRows = monthVisitRows
                .Where(x =>
                    customerBranchMap.ContainsKey(NormalizeCode(x.CustomerCode)) &&
                    customerBranchMap[NormalizeCode(x.CustomerCode)] == selectedBranchCode)
                .ToList();

            monthlyTargets = monthlyTargets
                .Where(x => NormalizeCode(x.BranchCode) == selectedBranchCode)
                .ToList();

            mtdSalesRows = mtdSalesRows
                .Where(x =>
                    customerBranchMap.ContainsKey(NormalizeCode(x.CustomerCode)) &&
                    customerBranchMap[NormalizeCode(x.CustomerCode)] == selectedBranchCode)
                .ToList();

            mtdVisitRows = mtdVisitRows
                .Where(x =>
                    customerBranchMap.ContainsKey(NormalizeCode(x.CustomerCode)) &&
                    customerBranchMap[NormalizeCode(x.CustomerCode)] == selectedBranchCode)
                .ToList();

            dailySalesRowsAfterMtd = dailySalesRowsAfterMtd
                .Where(x => IsDailySalesInBranch(x, selectedBranchCode, customerBranchMap, lineBranchMap))
                .ToList();

            dailyVisitRowsAfterMtd = dailyVisitRowsAfterMtd
                .Where(x =>
                    customerBranchMap.ContainsKey(NormalizeCode(x.CustomerCode)) &&
                    customerBranchMap[NormalizeCode(x.CustomerCode)] == selectedBranchCode)
                .ToList();
        }

        var salesToDateRows = new List<SalesToDateRow>();

        salesToDateRows.AddRange(mtdSalesRows.Select(x => new SalesToDateRow
        {
            LineCode = string.Empty,
            LineName = string.Empty,

            CityCode = x.CityCode,
            CityName = x.CityName,

            CustomerCode = x.CustomerCode,
            CustomerName = x.CustomerName,

            ProductCode = x.ProductCode,
            ProductName = x.ProductName,
            Unit = x.Unit,

            Quantity = x.Quantity,
            SalesAmount = x.TotalAfterTax > 0 ? x.TotalAfterTax : x.SalesAmount
        }));

        salesToDateRows.AddRange(dailySalesRowsAfterMtd.Select(x => new SalesToDateRow
        {
            LineCode = x.LineCode,
            LineName = x.LineName,

            CityCode = x.CityCode,
            CityName = x.CityName,

            CustomerCode = x.CustomerCode,
            CustomerName = x.CustomerName,

            ProductCode = x.ProductCode,
            ProductName = x.ProductName,
            Unit = x.Unit,

            Quantity = x.Quantity,
            SalesAmount = x.TotalAfterTax > 0 ? x.TotalAfterTax : x.SalesAmount
        }));

        var visitsToDateRows = new List<VisitToDateRow>();

        visitsToDateRows.AddRange(mtdVisitRows.Select(x => new VisitToDateRow
        {
            SupervisorName = x.SupervisorName,
            CityName = x.CityName,
            VisitCode = x.VisitCode,

            SalesRepCode = x.SalesRepCode,
            SalesRepName = x.SalesRepName,

            CustomerCode = x.CustomerCode,
            CustomerName = x.CustomerName,

            VisitStatus = x.VisitStatus,
            NegativeReason = x.NegativeReason,
            SuccessfulVisitValue = x.SuccessfulVisitValue,

            VisitStartTime = x.VisitStartTime,
            VisitEndTime = x.VisitEndTime,
            VisitDurationText = x.VisitDurationText
        }));

        visitsToDateRows.AddRange(dailyVisitRowsAfterMtd.Select(x => new VisitToDateRow
        {
            SupervisorName = x.SupervisorName,
            CityName = x.CityName,
            VisitCode = x.VisitCode,

            SalesRepCode = x.SalesRepCode,
            SalesRepName = x.SalesRepName,

            CustomerCode = x.CustomerCode,
            CustomerName = x.CustomerName,

            VisitStatus = x.VisitStatus,
            NegativeReason = x.NegativeReason,
            SuccessfulVisitValue = x.SuccessfulVisitValue,

            VisitStartTime = x.VisitStartTime,
            VisitEndTime = x.VisitEndTime,
            VisitDurationText = x.VisitDurationText
        }));

        var totalSalesToDate = salesToDateRows.Sum(x => x.SalesAmount);
        var totalQuantityToDate = salesToDateRows.Sum(x => x.Quantity);

        var totalVisitsToDate = visitsToDateRows.Count;
        var positiveVisitsToDate = visitsToDateRows.Count(IsPositiveVisit);
        var negativeVisitsToDate = totalVisitsToDate - positiveVisitsToDate;

        var totalVisitValueToDate = visitsToDateRows.Sum(x => x.SuccessfulVisitValue);

        var salesByLineMonth = salesToDateRows
            .Select(x => new
            {
                Sales = x,
                SalesDistrictCode = ResolveSalesDistrictCode(x.CustomerCode, x.LineCode, customerDistrictMap)
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.SalesDistrictCode))
            .GroupBy(x => x.SalesDistrictCode)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Sales.SalesAmount));

        var quantityByLineMonth = salesToDateRows
            .Select(x => new
            {
                Sales = x,
                SalesDistrictCode = ResolveSalesDistrictCode(x.CustomerCode, x.LineCode, customerDistrictMap)
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.SalesDistrictCode))
            .GroupBy(x => x.SalesDistrictCode)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.Sales.Quantity));

        var visitsByLineMonth = visitsToDateRows
            .Select(x => new
            {
                Visit = x,
                SalesDistrictCode = ResolveSalesDistrictCode(x.CustomerCode, null, customerDistrictMap)
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.SalesDistrictCode))
            .GroupBy(x => x.SalesDistrictCode)
            .ToDictionary(
                g => g.Key,
                g => g.Count());

        var allTargetAchievements = monthlyTargets
            .Select(target =>
            {
                var normalizedSalesDistrictCode = NormalizeCode(target.SalesDistrictCode);

                var actualSales = salesByLineMonth.TryGetValue(normalizedSalesDistrictCode, out var sales)
                    ? sales
                    : 0;

                var actualVisits = visitsByLineMonth.TryGetValue(normalizedSalesDistrictCode, out var visits)
                    ? visits
                    : 0;

                var remainingTarget = Math.Max(target.MonthlySalesTarget - actualSales, 0);

                var achievement = CalculatePercentage(actualSales, target.MonthlySalesTarget);
                var visitAchievement = CalculatePercentage(actualVisits, target.PlannedVisits);

                return new TargetAchievementDto
                {
                    BranchCode = target.BranchCode,
                    BranchName = target.BranchName,

                    SalesDistrictCode = target.SalesDistrictCode,
                    SalesDistrictName = target.SalesDistrictName,

                    MonthlyTarget = target.MonthlySalesTarget,
                    ActualSales = actualSales,
                    AchievementPercentage = achievement,
                    RemainingTarget = remainingTarget,
                    RequiredDailySales = Math.Round(remainingTarget / remainingDays, 2),

                    PlannedVisits = target.PlannedVisits,
                    ActualVisits = actualVisits,
                    VisitAchievementPercentage = visitAchievement,

                    Status = achievement >= 100
                        ? "Achieved"
                        : achievement >= 75
                            ? "On Track"
                            : achievement >= 50
                                ? "Needs Attention"
                                : "Critical"
                };
            })
            .ToList();

        var totalMonthlyTarget = allTargetAchievements.Sum(x => x.MonthlyTarget);
        var totalActualSalesToDate = totalSalesToDate;
        var totalRemainingTarget = Math.Max(totalMonthlyTarget - totalActualSalesToDate, 0);

        var totalPlannedVisits = allTargetAchievements.Sum(x => x.PlannedVisits);
        var totalActualVisits = allTargetAchievements.Sum(x => x.ActualVisits);

        var targetSummary = new TargetAchievementSummaryDto
        {
            TotalMonthlyTarget = totalMonthlyTarget,
            ActualSalesToDate = totalActualSalesToDate,
            AchievementPercentage = CalculatePercentage(totalActualSalesToDate, totalMonthlyTarget),
            RemainingTarget = totalRemainingTarget,
            RequiredDailySales = Math.Round(totalRemainingTarget / remainingDays, 2),

            PlannedVisits = totalPlannedVisits,
            ActualVisits = totalActualVisits,
            VisitAchievementPercentage = CalculatePercentage(totalActualVisits, totalPlannedVisits),

            CriticalLinesCount = allTargetAchievements.Count(x => x.Status == "Critical"),
            OnTrackLinesCount = allTargetAchievements.Count(x => x.Status == "On Track"),
            AchievedLinesCount = allTargetAchievements.Count(x => x.Status == "Achieved")
        };

        var lowestTargetAchievements = allTargetAchievements
            .OrderBy(x => x.AchievementPercentage)
            .ThenByDescending(x => x.MonthlyTarget)
            .Take(10)
            .ToList();

        var bestTargetAchievements = allTargetAchievements
            .Where(x => x.ActualSales > 0 || x.AchievementPercentage > 0)
            .OrderByDescending(x => x.AchievementPercentage)
            .ThenByDescending(x => x.ActualSales)
            .Take(10)
            .ToList();

        var branchPerformances = allTargetAchievements
    .GroupBy(x => new
    {
        BranchCode = NormalizeCode(x.BranchCode),
        BranchName = string.IsNullOrWhiteSpace(x.BranchName)
            ? NormalizeCode(x.BranchCode)
            : x.BranchName
    })
    .Select(g =>
    {
        var monthlyTarget = g.Sum(x => x.MonthlyTarget);
        var actualSales = g.Sum(x => x.ActualSales);
        var remainingTarget = Math.Max(monthlyTarget - actualSales, 0);

        var plannedVisits = g.Sum(x => x.PlannedVisits);
        var actualVisits = g.Sum(x => x.ActualVisits);

        return new BranchPerformanceDto
        {
            BranchCode = g.Key.BranchCode,
            BranchName = g.Key.BranchName ?? string.Empty,

            MonthlyTarget = monthlyTarget,
            ActualSales = actualSales,
            AchievementPercentage = CalculatePercentage(actualSales, monthlyTarget),
            RemainingTarget = remainingTarget,
            RequiredDailySales = Math.Round(remainingTarget / remainingDays, 2),

            PlannedVisits = plannedVisits,
            ActualVisits = actualVisits,
            VisitAchievementPercentage = CalculatePercentage(actualVisits, plannedVisits),

            TotalLinesCount = g.Count(),
            CriticalLinesCount = g.Count(x => x.Status == "Critical"),
            OnTrackLinesCount = g.Count(x => x.Status == "On Track"),
            AchievedLinesCount = g.Count(x => x.Status == "Achieved")
        };
    })
    .OrderByDescending(x => x.ActualSales)
    .ToList();

        var alerts = new List<CeoAlertDto>();

        // ==========================================
        // Target Achievement Alert
        // ==========================================

        if (targetSummary.TotalMonthlyTarget > 0 &&
            targetSummary.AchievementPercentage < 50)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "انخفاض تحقيق التارجت",

                Message =
                    $"نسبة تحقيق التارجت الحالية {targetSummary.AchievementPercentage:N2}% فقط، " +
                    $"والمطلوب تحقيق مبيعات يومية بقيمة {targetSummary.RequiredDailySales:N0} جنيه " +
                    $"للوصول إلى التارجت الشهري.",

                Severity = "Critical",
                Icon = "bi-exclamation-octagon"
            });
        }

        // ==========================================
        // Critical Sales Lines
        // ==========================================

        if (targetSummary.CriticalLinesCount > 0)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "خطوط مبيعات حرجة",

                Message =
                    $"يوجد {targetSummary.CriticalLinesCount} خط مبيعات " +
                    $"بنسبة تحقيق أقل من 50% من التارجت الشهري، " +
                    $"وتحتاج إلى متابعة مباشرة من إدارة المبيعات.",

                Severity = "Critical",
                Icon = "bi-bullseye"
            });
        }

        // ==========================================
        // Visit Achievement
        // ==========================================

        if (targetSummary.PlannedVisits > 0 &&
            targetSummary.VisitAchievementPercentage < 70)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "انخفاض تحقيق الزيارات",

                Message =
                    $"نسبة تحقيق الزيارات الحالية {targetSummary.VisitAchievementPercentage:N2}%، " +
                    $"بإجمالي {targetSummary.ActualVisits:N0} زيارة فعلية " +
                    $"من أصل {targetSummary.PlannedVisits:N0} زيارة مخططة.",

                Severity = "Warning",
                Icon = "bi-geo-alt"
            });
        }

        // ==========================================
        // Negative Visits
        // ==========================================

        if (negativeVisitsToDate > 0)
        {
            var negativeVisitPercentage =
                CalculatePercentage(
                    negativeVisitsToDate,
                    totalVisitsToDate);

            alerts.Add(new CeoAlertDto
            {
                Title = "وجود زيارات سلبية",

                Message =
                    $"تم تسجيل {negativeVisitsToDate:N0} زيارة سلبية حتى التاريخ المحدد، " +
                    $"وتمثل {negativeVisitPercentage:N2}% من إجمالي الزيارات.",

                Severity =
                    negativeVisitPercentage >= 25
                        ? "Warning"
                        : "Info",

                Icon = "bi-exclamation-triangle"
            });
        }

        // ==========================================
        // Top Negative Visit Reason
        // ==========================================

        var topNegativeReason = visitsToDateRows
            .Where(x => !IsPositiveVisit(x))
            .GroupBy(x =>
                string.IsNullOrWhiteSpace(x.NegativeReason)
                    ? "بدون سبب مسجل"
                    : x.NegativeReason.Trim())
            .Select(g => new
            {
                Reason = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        if (topNegativeReason != null)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "أكثر أسباب الزيارات السلبية",

                Message =
                    $"أكثر سبب متكرر للزيارات السلبية هو " +
                    $"«{topNegativeReason.Reason}» " +
                    $"بعدد {topNegativeReason.Count:N0} زيارة.",

                Severity = "Info",
                Icon = "bi-chat-left-text"
            });
        }

        // ==========================================
        // No Data
        // ==========================================

        if (totalSalesToDate == 0 &&
            totalVisitsToDate == 0)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "لا توجد بيانات تشغيلية",

                Message =
                    "لا توجد بيانات مبيعات أو زيارات مرفوعة " +
                    "للتاريخ والفرع المحددين.",

                Severity = "Warning",
                Icon = "bi-database-exclamation"
            });
        }




        var salesDistrictNameMap = monthlyTargets
            .GroupBy(x => NormalizeCode(x.SalesDistrictCode))
            .ToDictionary(
                g => g.Key,
                g => g.First().SalesDistrictName);

        var salesByLines = salesByLineMonth
            .Where(x => x.Value > 0)
            .Select(x =>
            {
                var lineName = salesDistrictNameMap.TryGetValue(x.Key, out var name)
                    ? name
                    : x.Key;

                var quantity = quantityByLineMonth.TryGetValue(x.Key, out var qty)
                    ? qty
                    : 0;

                return new SalesByLineDto
                {
                    LineCode = x.Key,
                    LineName = lineName,
                    TotalSales = x.Value,
                    TotalQuantity = quantity,
                    ContributionPercentage = CalculatePercentage(x.Value, totalSalesToDate)
                };
            })
            .OrderByDescending(x => x.TotalSales)
            .Take(10)
            .ToList();

        var visitsBySalesReps = visitsToDateRows
    .Where(x => !string.IsNullOrWhiteSpace(x.SalesRepCode))
    .GroupBy(x => new
    {
        SalesRepCode = NormalizeCode(x.SalesRepCode),
        x.SalesRepName
    })
    .Select(g =>
    {
        var totalVisits = g.Count();

        var positiveVisits =
            g.Count(IsPositiveVisit);

        var negativeVisits =
            totalVisits - positiveVisits;

        var totalVisitValue =
            g.Sum(x => x.SuccessfulVisitValue);

        var averageVisitValue =
            positiveVisits == 0
                ? 0
                : Math.Round(
                    totalVisitValue / positiveVisits,
                    2);

        return new VisitsBySalesRepDto
        {
            SalesRepCode =
                g.Key.SalesRepCode,

            SalesRepName =
                g.Key.SalesRepName,

            TotalVisits =
                totalVisits,

            PositiveVisits =
                positiveVisits,

            NegativeVisits =
                negativeVisits,

            PositiveVisitPercentage =
                CalculatePercentage(
                    positiveVisits,
                    totalVisits),

            NegativeVisitPercentage =
                CalculatePercentage(
                    negativeVisits,
                    totalVisits),

            TotalVisitValue =
                totalVisitValue,

            AverageVisitValue =
                averageVisitValue
        };
    })
    .OrderByDescending(x =>
        x.TotalVisitValue)
    .ThenByDescending(x =>
        x.PositiveVisitPercentage)
    .ToList();

        for (var i = 0; i < visitsBySalesReps.Count; i++)
        {
            visitsBySalesReps[i].Rank = i + 1;
        }

        // ==========================================
        // Customers With No Sales / Commercial Follow-up
        // ==========================================

        var salesByCustomer = salesToDateRows
            .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCode))
            .GroupBy(x => NormalizeCode(x.CustomerCode))
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.SalesAmount));

        var customerBranchNameMap = customers
            .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCode))
            .GroupBy(x => NormalizeCode(x.CustomerCode))
            .ToDictionary(
                g => g.Key,
                g => g.First().BranchName ?? string.Empty);

        var customerDistrictNameMap = _context.SalesAnalyticsCustomers
            .AsNoTracking()
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.CustomerCode))
            .Select(x => new
            {
                x.CustomerCode,
                x.SalesDistrictName
            })
            .ToList()
            .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCode))
            .GroupBy(x => NormalizeCode(x.CustomerCode))
            .ToDictionary(
                g => g.Key,
                g => g.First().SalesDistrictName ?? string.Empty);

        var bottomCustomers = visitsToDateRows
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.CustomerCode))
            .GroupBy(x => new
            {
                CustomerCode = NormalizeCode(x.CustomerCode),
                x.CustomerName
            })
            .Select(g =>
            {
                var customerCode = g.Key.CustomerCode;

                var totalCustomerVisits = g.Count();

                var positiveCustomerVisits =
                    g.Count(IsPositiveVisit);

                var negativeCustomerVisits =
                    totalCustomerVisits - positiveCustomerVisits;

                var salesValue =
                    salesByCustomer.TryGetValue(
                        customerCode,
                        out var customerSales)
                        ? customerSales
                        : 0;

                var successfulVisitValue =
                    g.Sum(x => x.SuccessfulVisitValue);

                var lastVisit = g
                    .OrderByDescending(x =>
                        x.VisitEndTime ??
                        x.VisitStartTime ??
                        DateTime.MinValue)
                    .First();

                var topCustomerNegativeReason = g
                    .Where(x => !IsPositiveVisit(x))
                    .GroupBy(x =>
                        string.IsNullOrWhiteSpace(x.NegativeReason)
                            ? "بدون سبب مسجل"
                            : x.NegativeReason.Trim())
                    .Select(x => new
                    {
                        Reason = x.Key,
                        Count = x.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefault();

                var branchCode =
                    customerBranchMap.TryGetValue(
                        customerCode,
                        out var mappedBranchCode)
                        ? mappedBranchCode
                        : string.Empty;

                var branchName =
                    customerBranchNameMap.TryGetValue(
                        customerCode,
                        out var mappedBranchName)
                        ? mappedBranchName
                        : string.Empty;

                var districtCode =
                    customerDistrictMap.TryGetValue(
                        customerCode,
                        out var mappedDistrictCode)
                        ? mappedDistrictCode
                        : string.Empty;

                var districtName =
                    customerDistrictNameMap.TryGetValue(
                        customerCode,
                        out var mappedDistrictName)
                        ? mappedDistrictName
                        : string.Empty;

                var hasNoSales = salesValue <= 0;

                var requiresCommercialFollowUp =
                    hasNoSales &&
                    totalCustomerVisits > 0;

                string riskLevel;

                if (hasNoSales &&
                    totalCustomerVisits >= 3 &&
                    negativeCustomerVisits == totalCustomerVisits)
                {
                    riskLevel = "حرج";
                }
                else if (hasNoSales &&
                         negativeCustomerVisits >= 2)
                {
                    riskLevel = "يحتاج متابعة عاجلة";
                }
                else
                {
                    riskLevel = "يحتاج متابعة";
                }

                return new BottomCustomerTodayDto
                {
                    CustomerCode = customerCode,
                    CustomerName = g.Key.CustomerName,

                    SalesRepCode =
                        NormalizeCode(lastVisit.SalesRepCode),

                    SalesRepName =
                        lastVisit.SalesRepName,

                    BranchCode = branchCode,
                    BranchName = branchName,

                    SalesDistrictCode = districtCode,
                    SalesDistrictName = districtName,

                    TotalVisits = totalCustomerVisits,

                    PositiveVisits = positiveCustomerVisits,
                    NegativeVisits = negativeCustomerVisits,

                    PositiveVisitPercentage =
                        CalculatePercentage(
                            positiveCustomerVisits,
                            totalCustomerVisits),

                    SalesValue = salesValue,

                    SuccessfulVisitValue =
                        successfulVisitValue,

                    NegativeReason =
                        topCustomerNegativeReason?.Reason,

                    VisitStatus =
                        lastVisit.VisitStatus,

                    HasNoSales =
                        hasNoSales,

                    RequiresCommercialFollowUp =
                        requiresCommercialFollowUp,

                    RiskLevel =
                        riskLevel
                };
            })
            // إحنا هنا عايزين العملاء اللي اتزاروا فعلاً
            // ولكن لم يحققوا أي مبيعات فعلية
            .Where(x =>
                x.HasNoSales &&
                x.RequiresCommercialFollowUp)

            // الأخطر يظهر أولاً
            .OrderByDescending(x =>
                x.RiskLevel == "حرج")

            .ThenByDescending(x =>
                x.NegativeVisits)

            .ThenByDescending(x =>
                x.TotalVisits)

            .ThenBy(x =>
                x.CustomerName)

            .Take(10)
            .ToList();

        var negativeVisitReasons = visitsToDateRows
            .Where(x => !IsPositiveVisit(x))
            .GroupBy(x => string.IsNullOrWhiteSpace(x.NegativeReason)
            ? "بدون سبب مسجل"
            : x.NegativeReason.Trim())
                    .Select(g => new NegativeVisitReasonDto
                    {
                        Reason = g.Key,
                        Count = g.Count(),
                        Percentage = CalculatePercentage(g.Count(), negativeVisitsToDate)
                    })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList();

        var topProducts = salesToDateRows
            .Where(x => !string.IsNullOrWhiteSpace(x.ProductName))
            .GroupBy(x => x.ProductName)
            .Select(g => new ProductPerformanceDto
            {
                ProductName = g.Key,
                TotalSales = g.Sum(x => x.SalesAmount),
                TotalQuantity = g.Sum(x => x.Quantity),
                ContributionPercentage = CalculatePercentage(g.Sum(x => x.SalesAmount), totalSalesToDate)
            })
            .OrderByDescending(x => x.TotalSales)
            .Take(10)
            .ToList();

        var dashboard = new SalesAnalyticsDashboardDto
        {
            ReportDate = date,

            Kpis = new SalesAnalyticsKpiDto
            {
                TotalSales = totalSalesToDate,
                TotalQuantity = totalQuantityToDate,
                TotalVisits = totalVisitsToDate,
                PositiveVisits = positiveVisitsToDate,
                NegativeVisits = negativeVisitsToDate,
                PositiveVisitPercentage = CalculatePercentage(positiveVisitsToDate, totalVisitsToDate),

                ActiveSalesReps = visitsToDateRows
            .Where(x => !string.IsNullOrWhiteSpace(x.SalesRepCode))
            .Select(x => x.SalesRepCode)
            .Distinct()
            .Count(),

                AverageVisitValue = positiveVisitsToDate == 0
            ? 0
            : Math.Round(totalVisitValueToDate / positiveVisitsToDate, 2)
            },

            TargetSummary = targetSummary,
            Alerts = alerts,
            BranchPerformances = branchPerformances,

            TargetAchievements = lowestTargetAchievements,
            LowestTargetAchievements = lowestTargetAchievements,
            BestTargetAchievements = bestTargetAchievements,

            SalesByLines = salesByLines,
            VisitsBySalesReps = visitsBySalesReps,
            BottomCustomersToday = bottomCustomers,
            NegativeVisitReasons = negativeVisitReasons,
            TopProducts = topProducts
        };

        return dashboard;
    }

    public List<NoSalesCustomerDto> GetNoSalesCustomers(
    DateTime reportDate,
    string? branchCode = null)
    {
        var date = reportDate.Date;
        var nextDate = date.AddDays(1);

        var monthStart = new DateTime(
            date.Year,
            date.Month,
            1);

        var selectedBranchCode =
            NormalizeCode(branchCode);

        // ==========================================
        // Daily Rows For Selected Month
        // ==========================================

        var monthSalesRows =
            _context.SalesAnalyticsDailySalesReports
                .AsNoTracking()
                .Where(x =>
                    x.ReportDate >= monthStart &&
                    x.ReportDate < nextDate)
                .ToList();

        var monthVisitRows =
            _context.SalesAnalyticsDailyVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.ReportDate >= monthStart &&
                    x.ReportDate < nextDate)
                .ToList();

        // ==========================================
        // Sales Ranges
        // ==========================================

        var salesRangeGroups =
            _context.SalesAnalyticsMtdSalesReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.FromDate <= date &&
                    x.ToDate <= date)
                .GroupBy(x => new
                {
                    x.UploadBatchId,
                    x.FromDate,
                    x.ToDate
                })
                .Select(g => new DateRangeSelection
                {
                    UploadBatchId = g.Key.UploadBatchId,
                    FromDate = g.Key.FromDate.Date,
                    ToDate = g.Key.ToDate.Date
                })
                .ToList();

        var selectedSalesRanges =
            SelectNonOverlappingRanges(
                salesRangeGroups);

        var selectedSalesBatchIds =
            selectedSalesRanges
                .Select(x => x.UploadBatchId)
                .ToHashSet();

        var mtdSalesRows =
            selectedSalesBatchIds.Count == 0
                ? new List<SalesAnalyticsMtdSalesReportEntity>()
                : _context.SalesAnalyticsMtdSalesReports
                    .AsNoTracking()
                    .Where(x =>
                        selectedSalesBatchIds.Contains(
                            x.UploadBatchId))
                    .ToList();

        var dailySalesRowsAfterMtd =
            monthSalesRows
                .Where(x =>
                    !IsDateCoveredByRanges(
                        x.ReportDate.Date,
                        selectedSalesRanges))
                .ToList();

        // ==========================================
        // Visit Ranges
        // ==========================================

        var visitRangeGroups =
            _context.SalesAnalyticsMtdVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.FromDate <= date &&
                    x.ToDate <= date)
                .GroupBy(x => new
                {
                    x.UploadBatchId,
                    x.FromDate,
                    x.ToDate
                })
                .Select(g => new DateRangeSelection
                {
                    UploadBatchId = g.Key.UploadBatchId,
                    FromDate = g.Key.FromDate.Date,
                    ToDate = g.Key.ToDate.Date
                })
                .ToList();

        var selectedVisitRanges =
            SelectNonOverlappingRanges(
                visitRangeGroups);

        var selectedVisitBatchIds =
            selectedVisitRanges
                .Select(x => x.UploadBatchId)
                .ToHashSet();

        var mtdVisitRows =
            selectedVisitBatchIds.Count == 0
                ? new List<SalesAnalyticsMtdVisitReportEntity>()
                : _context.SalesAnalyticsMtdVisitReports
                    .AsNoTracking()
                    .Where(x =>
                        selectedVisitBatchIds.Contains(
                            x.UploadBatchId))
                    .ToList();

        var dailyVisitRowsAfterMtd =
            monthVisitRows
                .Where(x =>
                    !IsDateCoveredByRanges(
                        x.ReportDate.Date,
                        selectedVisitRanges))
                .ToList();

        // ==========================================
        // Customer Master
        // ==========================================

        var customers =
            _context.SalesAnalyticsCustomers
                .AsNoTracking()
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode))
                .Select(x => new
                {
                    x.CustomerCode,
                    x.BranchCode,
                    x.BranchName,
                    x.SalesDistrictCode,
                    x.SalesDistrictName
                })
                .ToList();

        var customerMasterMap =
            customers
                .GroupBy(x =>
                    NormalizeCode(x.CustomerCode))
                .ToDictionary(
                    g => g.Key,
                    g => g.First());

        // ==========================================
        // Branch Filter
        // ==========================================

        if (!string.IsNullOrWhiteSpace(
            selectedBranchCode))
        {
            mtdSalesRows =
                mtdSalesRows
                    .Where(x =>
                    {
                        var customerCode =
                            NormalizeCode(
                                x.CustomerCode);

                        return customerMasterMap
                                   .TryGetValue(
                                       customerCode,
                                       out var customer)
                               &&
                               NormalizeCode(
                                   customer.BranchCode)
                               ==
                               selectedBranchCode;
                    })
                    .ToList();

            dailySalesRowsAfterMtd =
                dailySalesRowsAfterMtd
                    .Where(x =>
                    {
                        var customerCode =
                            NormalizeCode(
                                x.CustomerCode);

                        return customerMasterMap
                                   .TryGetValue(
                                       customerCode,
                                       out var customer)
                               &&
                               NormalizeCode(
                                   customer.BranchCode)
                               ==
                               selectedBranchCode;
                    })
                    .ToList();

            mtdVisitRows =
                mtdVisitRows
                    .Where(x =>
                    {
                        var customerCode =
                            NormalizeCode(
                                x.CustomerCode);

                        return customerMasterMap
                                   .TryGetValue(
                                       customerCode,
                                       out var customer)
                               &&
                               NormalizeCode(
                                   customer.BranchCode)
                               ==
                               selectedBranchCode;
                    })
                    .ToList();

            dailyVisitRowsAfterMtd =
                dailyVisitRowsAfterMtd
                    .Where(x =>
                    {
                        var customerCode =
                            NormalizeCode(
                                x.CustomerCode);

                        return customerMasterMap
                                   .TryGetValue(
                                       customerCode,
                                       out var customer)
                               &&
                               NormalizeCode(
                                   customer.BranchCode)
                               ==
                               selectedBranchCode;
                    })
                    .ToList();
        }

        // ==========================================
        // Actual Sales By Customer
        // ==========================================

        var salesByCustomer =
            mtdSalesRows
                .Select(x => new
                {
                    CustomerCode =
                        NormalizeCode(
                            x.CustomerCode),

                    Sales =
                        x.TotalAfterTax > 0
                            ? x.TotalAfterTax
                            : x.SalesAmount
                })
                .Concat(
                    dailySalesRowsAfterMtd
                        .Select(x => new
                        {
                            CustomerCode =
                                NormalizeCode(
                                    x.CustomerCode),

                            Sales =
                                x.TotalAfterTax > 0
                                    ? x.TotalAfterTax
                                    : x.SalesAmount
                        }))
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode))
                .GroupBy(x =>
                    x.CustomerCode)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x => x.Sales));

        // ==========================================
        // Visits To Date
        // ==========================================

        var visitsToDateRows =
            new List<VisitToDateRow>();

        visitsToDateRows.AddRange(
            mtdVisitRows.Select(x =>
                new VisitToDateRow
                {
                    SupervisorName =
                        x.SupervisorName,

                    CityName =
                        x.CityName,

                    VisitCode =
                        x.VisitCode,

                    SalesRepCode =
                        x.SalesRepCode,

                    SalesRepName =
                        x.SalesRepName,

                    CustomerCode =
                        x.CustomerCode,

                    CustomerName =
                        x.CustomerName,

                    VisitStatus =
                        x.VisitStatus,

                    NegativeReason =
                        x.NegativeReason,

                    SuccessfulVisitValue =
                        x.SuccessfulVisitValue,

                    VisitStartTime =
                        x.VisitStartTime,

                    VisitEndTime =
                        x.VisitEndTime,

                    VisitDurationText =
                        x.VisitDurationText
                }));

        visitsToDateRows.AddRange(
            dailyVisitRowsAfterMtd.Select(x =>
                new VisitToDateRow
                {
                    SupervisorName =
                        x.SupervisorName,

                    CityName =
                        x.CityName,

                    VisitCode =
                        x.VisitCode,

                    SalesRepCode =
                        x.SalesRepCode,

                    SalesRepName =
                        x.SalesRepName,

                    CustomerCode =
                        x.CustomerCode,

                    CustomerName =
                        x.CustomerName,

                    VisitStatus =
                        x.VisitStatus,

                    NegativeReason =
                        x.NegativeReason,

                    SuccessfulVisitValue =
                        x.SuccessfulVisitValue,

                    VisitStartTime =
                        x.VisitStartTime,

                    VisitEndTime =
                        x.VisitEndTime,

                    VisitDurationText =
                        x.VisitDurationText
                }));

        // ==========================================
        // Customers With Visits But Zero Actual Sales
        // ==========================================

        var result =
            visitsToDateRows
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode))
                .GroupBy(x => new
                {
                    CustomerCode =
                        NormalizeCode(
                            x.CustomerCode),

                    x.CustomerName
                })
                .Select(g =>
                {
                    var customerCode =
                        g.Key.CustomerCode;

                    var totalVisits =
                        g.Count();

                    var positiveVisits =
                        g.Count(IsPositiveVisit);

                    var negativeVisits =
                        totalVisits -
                        positiveVisits;

                    var salesValue =
                        salesByCustomer.TryGetValue(
                            customerCode,
                            out var sales)
                            ? sales
                            : 0;

                    var lastVisit =
                        g.OrderByDescending(x =>
                                x.VisitEndTime ??
                                x.VisitStartTime ??
                                DateTime.MinValue)
                            .First();

                    var topNegativeReason =
                        g.Where(x =>
                                !IsPositiveVisit(x))
                            .GroupBy(x =>
                                string.IsNullOrWhiteSpace(
                                    x.NegativeReason)
                                    ? "بدون سبب مسجل"
                                    : x.NegativeReason.Trim())
                            .Select(x => new
                            {
                                Reason = x.Key,
                                Count = x.Count()
                            })
                            .OrderByDescending(x =>
                                x.Count)
                            .FirstOrDefault();

                    customerMasterMap.TryGetValue(
                        customerCode,
                        out var customerMaster);

                    string riskLevel;

                    if (
                        totalVisits >= 3 &&
                        negativeVisits ==
                        totalVisits)
                    {
                        riskLevel = "حرج";
                    }
                    else if (
                        negativeVisits >= 2)
                    {
                        riskLevel =
                            "يحتاج متابعة عاجلة";
                    }
                    else
                    {
                        riskLevel =
                            "يحتاج متابعة";
                    }

                    return new NoSalesCustomerDto
                    {
                        CustomerCode =
                            customerCode,

                        CustomerName =
                            g.Key.CustomerName,

                        BranchCode =
                            customerMaster?.BranchCode
                            ?? string.Empty,

                        BranchName =
                            customerMaster?.BranchName
                            ?? string.Empty,

                        SalesDistrictCode =
                            customerMaster
                                ?.SalesDistrictCode
                            ?? string.Empty,

                        SalesDistrictName =
                            customerMaster
                                ?.SalesDistrictName
                            ?? string.Empty,

                        SalesRepCode =
                            NormalizeCode(
                                lastVisit.SalesRepCode),

                        SalesRepName =
                            lastVisit.SalesRepName,

                        TotalVisits =
                            totalVisits,

                        PositiveVisits =
                            positiveVisits,

                        NegativeVisits =
                            negativeVisits,

                        PositiveVisitPercentage =
                            CalculatePercentage(
                                positiveVisits,
                                totalVisits),

                        SalesValue =
                            salesValue,

                        TopNegativeReason =
                            topNegativeReason
                                ?.Reason,

                        RiskLevel =
                            riskLevel
                    };
                })

                // تمت زيارته فعلاً
                // لكن المبيعات الفعلية = صفر
                .Where(x =>
                    x.TotalVisits > 0 &&
                    x.SalesValue <= 0)

                // الحالات الأخطر أولاً
                .OrderBy(x =>
                    x.RiskLevel == "حرج"
                        ? 1
                        : x.RiskLevel ==
                          "يحتاج متابعة عاجلة"
                            ? 2
                            : 3)

                .ThenByDescending(x =>
                    x.NegativeVisits)

                .ThenByDescending(x =>
                    x.TotalVisits)

                .ThenBy(x =>
                    x.CustomerName)

                // مهم جداً:
                // لا يوجد Take هنا
                // نرجع كل العملاء
                .ToList();

        return result;
    }

    public DateTime? GetLatestReportDate()
    {
        var latestSalesDate = _context.SalesAnalyticsDailySalesReports
            .AsNoTracking()
            .Select(x => (DateTime?)x.ReportDate)
            .Max();

        var latestVisitsDate = _context.SalesAnalyticsDailyVisitReports
            .AsNoTracking()
            .Select(x => (DateTime?)x.ReportDate)
            .Max();

        var latestMtdSalesDate = _context.SalesAnalyticsMtdSalesReports
            .AsNoTracking()
            .Select(x => (DateTime?)x.ToDate)
            .Max();

        var latestMtdVisitsDate = _context.SalesAnalyticsMtdVisitReports
            .AsNoTracking()
            .Select(x => (DateTime?)x.ToDate)
            .Max();

        var dates = new[]
            {
                latestSalesDate,
                latestVisitsDate,
                latestMtdSalesDate,
                latestMtdVisitsDate
            }
            .Where(x => x.HasValue)
            .Select(x => x!.Value.Date)
            .ToList();

        return dates.Any()
            ? dates.Max()
            : null;
    }

    public List<BranchOptionDto> GetBranchOptions()
    {
        var rows = _context.SalesAnalyticsCustomers
            .AsNoTracking()
            .Where(x => !string.IsNullOrWhiteSpace(x.BranchCode))
            .Select(x => new
            {
                x.BranchCode,
                x.BranchName
            })
            .ToList();

        return rows
            .GroupBy(x => NormalizeCode(x.BranchCode))
            .Select(g =>
            {
                var first = g.First();

                return new BranchOptionDto
                {
                    BranchCode = first.BranchCode ?? string.Empty,
                    BranchName = string.IsNullOrWhiteSpace(first.BranchName)
                        ? first.BranchCode ?? string.Empty
                        : first.BranchName
                };
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.BranchCode))
            .OrderBy(x => x.BranchName)
            .ToList();
    }

    public List<SalesRepEffectivenessDto> GetSalesRepEffectiveness(
    DateTime reportDate,
    string? branchCode = null)
    {
        var date = reportDate.Date;
        var nextDate = date.AddDays(1);

        var monthStart = new DateTime(
            date.Year,
            date.Month,
            1);

        var selectedBranchCode =
            NormalizeCode(branchCode);

        // =========================================================
        // Customer Master
        // =========================================================

        var customers =
            _context.SalesAnalyticsCustomers
                .AsNoTracking()
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.CustomerCode))
                .Select(x => new
                {
                    x.CustomerCode,
                    x.BranchCode,
                    x.BranchName
                })
                .ToList();

        var customerMasterMap =
            customers
                .GroupBy(x =>
                    NormalizeCode(x.CustomerCode))
                .ToDictionary(
                    g => g.Key,
                    g => g.First());

        // =========================================================
        // Daily Visits For Selected Month
        // =========================================================

        var monthVisitRows =
            _context.SalesAnalyticsDailyVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.ReportDate >= monthStart &&
                    x.ReportDate < nextDate)
                .ToList();

        // =========================================================
        // Visits Ranges
        // =========================================================

        var visitRangeGroups =
            _context.SalesAnalyticsMtdVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.FromDate <= date &&
                    x.ToDate <= date)
                .GroupBy(x => new
                {
                    x.UploadBatchId,
                    x.FromDate,
                    x.ToDate
                })
                .Select(g => new DateRangeSelection
                {
                    UploadBatchId = g.Key.UploadBatchId,
                    FromDate = g.Key.FromDate.Date,
                    ToDate = g.Key.ToDate.Date
                })
                .ToList();

        var selectedVisitRanges =
            SelectNonOverlappingRanges(
                visitRangeGroups);

        var selectedVisitBatchIds =
            selectedVisitRanges
                .Select(x => x.UploadBatchId)
                .ToHashSet();

        var mtdVisitRows =
            selectedVisitBatchIds.Count == 0
                ? new List<SalesAnalyticsMtdVisitReportEntity>()
                : _context.SalesAnalyticsMtdVisitReports
                    .AsNoTracking()
                    .Where(x =>
                        selectedVisitBatchIds.Contains(
                            x.UploadBatchId))
                    .ToList();

        var dailyVisitRows =
            monthVisitRows
                .Where(x =>
                    !IsDateCoveredByRanges(
                        x.ReportDate.Date,
                        selectedVisitRanges))
                .ToList();

        // =========================================================
        // Branch Filter
        // =========================================================

        if (!string.IsNullOrWhiteSpace(selectedBranchCode))
        {
            mtdVisitRows =
                mtdVisitRows
                    .Where(x =>
                    {
                        var customerCode =
                            NormalizeCode(x.CustomerCode);

                        return customerMasterMap.TryGetValue(
                                   customerCode,
                                   out var customer)
                               &&
                               NormalizeCode(customer.BranchCode)
                               == selectedBranchCode;
                    })
                    .ToList();

            dailyVisitRows =
                dailyVisitRows
                    .Where(x =>
                    {
                        var customerCode =
                            NormalizeCode(x.CustomerCode);

                        return customerMasterMap.TryGetValue(
                                   customerCode,
                                   out var customer)
                               &&
                               NormalizeCode(customer.BranchCode)
                               == selectedBranchCode;
                    })
                    .ToList();
        }

        // =========================================================
        // Visits To Date
        // =========================================================

        var visitsToDateRows =
            new List<VisitToDateRow>();

        visitsToDateRows.AddRange(
            mtdVisitRows.Select(x =>
                new VisitToDateRow
                {
                    SupervisorName = x.SupervisorName,
                    CityName = x.CityName,
                    VisitCode = x.VisitCode,

                    SalesRepCode = x.SalesRepCode,
                    SalesRepName = x.SalesRepName,

                    CustomerCode = x.CustomerCode,
                    CustomerName = x.CustomerName,

                    VisitStatus = x.VisitStatus,
                    NegativeReason = x.NegativeReason,

                    SuccessfulVisitValue =
                        x.SuccessfulVisitValue,

                    VisitStartTime =
                        x.VisitStartTime,

                    VisitEndTime =
                        x.VisitEndTime,

                    VisitDurationText =
                        x.VisitDurationText
                }));

        visitsToDateRows.AddRange(
            dailyVisitRows.Select(x =>
                new VisitToDateRow
                {
                    SupervisorName = x.SupervisorName,
                    CityName = x.CityName,
                    VisitCode = x.VisitCode,

                    SalesRepCode = x.SalesRepCode,
                    SalesRepName = x.SalesRepName,

                    CustomerCode = x.CustomerCode,
                    CustomerName = x.CustomerName,

                    VisitStatus = x.VisitStatus,
                    NegativeReason = x.NegativeReason,

                    SuccessfulVisitValue =
                        x.SuccessfulVisitValue,

                    VisitStartTime =
                        x.VisitStartTime,

                    VisitEndTime =
                        x.VisitEndTime,

                    VisitDurationText =
                        x.VisitDurationText
                }));

        // =========================================================
        // Valid Visits
        // =========================================================

        var validVisits =
            visitsToDateRows
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.SalesRepCode) &&
                    !string.IsNullOrWhiteSpace(x.CustomerCode))
                .ToList();

        // =========================================================
        // Sales Rep Field Performance
        // =========================================================

        var result =
            validVisits
                .GroupBy(x => new
                {
                    SalesRepCode =
                        NormalizeCode(x.SalesRepCode),

                    SalesRepName =
                        x.SalesRepName?.Trim()
                        ?? string.Empty
                })
                .Select(g =>
                {
                    var visitedCustomerCodes =
                        g.Select(x =>
                                NormalizeCode(x.CustomerCode))
                            .Where(x =>
                                !string.IsNullOrWhiteSpace(x))
                            .Distinct()
                            .ToList();

                    var visitedCustomers =
                        visitedCustomerCodes.Count;

                    var totalVisits =
                        g.Count();

                    var positiveVisits =
                        g.Count(IsPositiveVisit);

                    var negativeVisits =
                        totalVisits - positiveVisits;

                    var positiveVisitRate =
                        CalculatePercentage(
                            positiveVisits,
                            totalVisits);

                    var negativeVisitRate =
                        CalculatePercentage(
                            negativeVisits,
                            totalVisits);

                    var averageVisitsPerCustomer =
                        visitedCustomers == 0
                            ? 0
                            : Math.Round(
                                (decimal)totalVisits /
                                visitedCustomers,
                                2);

                    var totalVisitValue =
                        g.Sum(x =>
                            x.SuccessfulVisitValue);

                    // =================================================
                    // Derive Main Branch From Rep's Visited Customers
                    // =================================================

                    var branch =
                        visitedCustomerCodes
                            .Select(customerCode =>
                            {
                                customerMasterMap.TryGetValue(
                                    customerCode,
                                    out var customer);

                                return customer;
                            })
                            .Where(x => x != null)
                            .GroupBy(x => new
                            {
                                x!.BranchCode,
                                x.BranchName
                            })
                            .OrderByDescending(x =>
                                x.Count())
                            .Select(x =>
                                x.Key)
                            .FirstOrDefault();

                    return new SalesRepEffectivenessDto
                    {
                        SalesRepCode =
                            g.Key.SalesRepCode,

                        SalesRepName =
                            g.Key.SalesRepName,

                        BranchCode =
                            branch?.BranchCode
                            ?? string.Empty,

                        BranchName =
                            branch?.BranchName
                            ?? string.Empty,

                        VisitedCustomers =
                            visitedCustomers,

                        TotalVisits =
                            totalVisits,

                        PositiveVisits =
                            positiveVisits,

                        NegativeVisits =
                            negativeVisits,

                        PositiveVisitRate =
                            positiveVisitRate,

                        NegativeVisitRate =
                            negativeVisitRate,

                        AverageVisitsPerCustomer =
                            averageVisitsPerCustomer,

                        TotalVisitValue =
                            totalVisitValue
                    };
                })

                .Where(x =>
                    x.TotalVisits > 0 &&
                    x.VisitedCustomers > 0)

                // =================================================
                // Ranking:
                // 1. Positive Visit Rate
                // 2. Total Visit Value
                // 3. Positive Visits
                // =================================================

                .OrderByDescending(x =>
                    x.PositiveVisitRate)

                .ThenByDescending(x =>
                    x.TotalVisitValue)

                .ThenByDescending(x =>
                    x.PositiveVisits)

                .ToList();

        // =========================================================
        // Rank
        // =========================================================

        for (var i = 0;
             i < result.Count;
             i++)
        {
            result[i].Rank = i + 1;
        }

        return result;
    }

    public List<BranchPerformanceAnalysisDto> GetBranchPerformanceAnalysis(
     DateTime reportDate,
     string? branchCode = null)
    {
        var date = reportDate.Date;

        var nextDate =
            date.AddDays(1);

        var monthStart =
            new DateTime(
                date.Year,
                date.Month,
                1);

        var selectedBranchCode =
            NormalizeCode(branchCode);

        var daysInMonth =
            DateTime.DaysInMonth(
                date.Year,
                date.Month);

        var elapsedDays =
            date.Day;

        var remainingDays =
            daysInMonth - elapsedDays;

        var expectedAchievementPercentage =
            daysInMonth == 0
                ? 0
                : Math.Round(
                    (decimal)elapsedDays /
                    daysInMonth *
                    100,
                    2);

        // =========================================================
        // Customer Master
        // =========================================================

        var customers =
            _context.SalesAnalyticsCustomers
                .AsNoTracking()
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode))
                .Select(x => new
                {
                    x.CustomerCode,
                    x.BranchCode,
                    x.BranchName
                })
                .ToList();

        var customerMasterMap =
            customers
                .GroupBy(x =>
                    NormalizeCode(
                        x.CustomerCode))
                .ToDictionary(
                    g => g.Key,
                    g => g.First());

        // =========================================================
        // Monthly Targets
        // =========================================================

        var monthlyTargets =
            _context.SalesDistrictMonthlyTargets
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month)
                .ToList();

        if (!string.IsNullOrWhiteSpace(
            selectedBranchCode))
        {
            monthlyTargets =
                monthlyTargets
                    .Where(x =>
                        NormalizeCode(
                            x.BranchCode)
                        ==
                        selectedBranchCode)
                    .ToList();
        }

        // =========================================================
        // Daily Rows For Selected Month
        // =========================================================

        var monthSalesRows =
            _context.SalesAnalyticsDailySalesReports
                .AsNoTracking()
                .Where(x =>
                    x.ReportDate >= monthStart &&
                    x.ReportDate < nextDate)
                .ToList();

        var monthVisitRows =
            _context.SalesAnalyticsDailyVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.ReportDate >= monthStart &&
                    x.ReportDate < nextDate)
                .ToList();

        // =========================================================
        // Sales Ranges
        // =========================================================

        var salesRangeGroups =
            _context.SalesAnalyticsMtdSalesReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.FromDate <= date &&
                    x.ToDate <= date)
                .GroupBy(x => new
                {
                    x.UploadBatchId,
                    x.FromDate,
                    x.ToDate
                })
                .Select(g => new DateRangeSelection
                {
                    UploadBatchId = g.Key.UploadBatchId,
                    FromDate = g.Key.FromDate.Date,
                    ToDate = g.Key.ToDate.Date
                })
                .ToList();

        var selectedSalesRanges =
            SelectNonOverlappingRanges(
                salesRangeGroups);

        var selectedSalesBatchIds =
            selectedSalesRanges
                .Select(x => x.UploadBatchId)
                .ToHashSet();

        var mtdSalesRows =
            selectedSalesBatchIds.Count == 0
                ? new List<SalesAnalyticsMtdSalesReportEntity>()
                : _context.SalesAnalyticsMtdSalesReports
                    .AsNoTracking()
                    .Where(x =>
                        selectedSalesBatchIds.Contains(
                            x.UploadBatchId))
                    .ToList();

        var dailySalesRows =
            monthSalesRows
                .Where(x =>
                    !IsDateCoveredByRanges(
                        x.ReportDate.Date,
                        selectedSalesRanges))
                .ToList();

        // =========================================================
        // Visit Ranges
        // =========================================================

        var visitRangeGroups =
            _context.SalesAnalyticsMtdVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.FromDate <= date &&
                    x.ToDate <= date)
                .GroupBy(x => new
                {
                    x.UploadBatchId,
                    x.FromDate,
                    x.ToDate
                })
                .Select(g => new DateRangeSelection
                {
                    UploadBatchId = g.Key.UploadBatchId,
                    FromDate = g.Key.FromDate.Date,
                    ToDate = g.Key.ToDate.Date
                })
                .ToList();

        var selectedVisitRanges =
            SelectNonOverlappingRanges(
                visitRangeGroups);

        var selectedVisitBatchIds =
            selectedVisitRanges
                .Select(x => x.UploadBatchId)
                .ToHashSet();

        var mtdVisitRows =
            selectedVisitBatchIds.Count == 0
                ? new List<SalesAnalyticsMtdVisitReportEntity>()
                : _context.SalesAnalyticsMtdVisitReports
                    .AsNoTracking()
                    .Where(x =>
                        selectedVisitBatchIds.Contains(
                            x.UploadBatchId))
                    .ToList();

        var dailyVisitRows =
            monthVisitRows
                .Where(x =>
                    !IsDateCoveredByRanges(
                        x.ReportDate.Date,
                        selectedVisitRanges))
                .ToList();

        // =========================================================
        // Normalize Sales
        // =========================================================

        var salesRows =
            mtdSalesRows
                .Select(x => new
                {
                    CustomerCode =
                        NormalizeCode(
                            x.CustomerCode),

                    SalesValue =
                        x.TotalAfterTax > 0
                            ? x.TotalAfterTax
                            : x.SalesAmount
                })
                .Concat(
                    dailySalesRows
                        .Select(x => new
                        {
                            CustomerCode =
                                NormalizeCode(
                                    x.CustomerCode),

                            SalesValue =
                                x.TotalAfterTax > 0
                                    ? x.TotalAfterTax
                                    : x.SalesAmount
                        }))
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode))
                .ToList();

        // =========================================================
        // Normalize Visits
        // =========================================================

        var visitRows =
            new List<VisitToDateRow>();

        visitRows.AddRange(
            mtdVisitRows.Select(x =>
                new VisitToDateRow
                {
                    SupervisorName =
                        x.SupervisorName,

                    CityName =
                        x.CityName,

                    VisitCode =
                        x.VisitCode,

                    SalesRepCode =
                        x.SalesRepCode,

                    SalesRepName =
                        x.SalesRepName,

                    CustomerCode =
                        x.CustomerCode,

                    CustomerName =
                        x.CustomerName,

                    VisitStatus =
                        x.VisitStatus,

                    NegativeReason =
                        x.NegativeReason,

                    SuccessfulVisitValue =
                        x.SuccessfulVisitValue,

                    VisitStartTime =
                        x.VisitStartTime,

                    VisitEndTime =
                        x.VisitEndTime,

                    VisitDurationText =
                        x.VisitDurationText
                }));

        visitRows.AddRange(
            dailyVisitRows.Select(x =>
                new VisitToDateRow
                {
                    SupervisorName =
                        x.SupervisorName,

                    CityName =
                        x.CityName,

                    VisitCode =
                        x.VisitCode,

                    SalesRepCode =
                        x.SalesRepCode,

                    SalesRepName =
                        x.SalesRepName,

                    CustomerCode =
                        x.CustomerCode,

                    CustomerName =
                        x.CustomerName,

                    VisitStatus =
                        x.VisitStatus,

                    NegativeReason =
                        x.NegativeReason,

                    SuccessfulVisitValue =
                        x.SuccessfulVisitValue,

                    VisitStartTime =
                        x.VisitStartTime,

                    VisitEndTime =
                        x.VisitEndTime,

                    VisitDurationText =
                        x.VisitDurationText
                }));

        // =========================================================
        // Branch Filter
        // =========================================================

        if (!string.IsNullOrWhiteSpace(
            selectedBranchCode))
        {
            salesRows =
                salesRows
                    .Where(x =>
                    {
                        return
                            customerMasterMap.TryGetValue(
                                x.CustomerCode,
                                out var customer)
                            &&
                            NormalizeCode(
                                customer.BranchCode)
                            ==
                            selectedBranchCode;
                    })
                    .ToList();

            visitRows =
                visitRows
                    .Where(x =>
                    {
                        var customerCode =
                            NormalizeCode(
                                x.CustomerCode);

                        return
                            customerMasterMap.TryGetValue(
                                customerCode,
                                out var customer)
                            &&
                            NormalizeCode(
                                customer.BranchCode)
                            ==
                            selectedBranchCode;
                    })
                    .ToList();
        }

        // =========================================================
        // Sales By Branch
        // =========================================================

        var salesByBranch =
            salesRows
                .Select(x =>
                {
                    customerMasterMap.TryGetValue(
                        x.CustomerCode,
                        out var customer);

                    return new
                    {
                        BranchCode =
                            customer == null
                                ? string.Empty
                                : NormalizeCode(
                                    customer.BranchCode),

                        SalesValue =
                            x.SalesValue
                    };
                })
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.BranchCode))
                .GroupBy(x =>
                    x.BranchCode)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x =>
                        x.SalesValue));

        // =========================================================
        // Visits By Branch
        // =========================================================

        var visitsWithBranch =
            visitRows
                .Select(x =>
                {
                    var customerCode =
                        NormalizeCode(
                            x.CustomerCode);

                    customerMasterMap.TryGetValue(
                        customerCode,
                        out var customer);

                    return new
                    {
                        Visit = x,

                        BranchCode =
                            customer == null
                                ? string.Empty
                                : NormalizeCode(
                                    customer.BranchCode)
                    };
                })
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.BranchCode))
                .ToList();

        // =========================================================
        // Target By Branch
        // =========================================================

        var targetByBranch =
            monthlyTargets
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.BranchCode))
                .GroupBy(x =>
                    NormalizeCode(
                        x.BranchCode))
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        BranchCode =
                            g.Key,

                        BranchName =
                            g.Select(x =>
                                    x.BranchName)
                                .FirstOrDefault(x =>
                                    !string.IsNullOrWhiteSpace(
                                        x))
                            ?? string.Empty,

                        MonthlyTarget =
                            g.Sum(x =>
                                x.MonthlySalesTarget),

                        PlannedVisits =
                            g.Sum(x =>
                                x.PlannedVisits)
                    });

        // =========================================================
        // Branches To Analyze
        // =========================================================

        var branchCodes =
            targetByBranch.Keys
                .Concat(
                    salesByBranch.Keys)
                .Concat(
                    visitsWithBranch
                        .Select(x =>
                            x.BranchCode))
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x))
                .Distinct()
                .ToList();

        // =========================================================
        // Build Branch Performance
        // =========================================================

        var result =
            new List<BranchPerformanceAnalysisDto>();

        foreach (var currentBranchCode
                 in branchCodes)
        {
            targetByBranch.TryGetValue(
                currentBranchCode,
                out var target);

            salesByBranch.TryGetValue(
                currentBranchCode,
                out var actualSales);

            var branchVisits =
                visitsWithBranch
                    .Where(x =>
                        x.BranchCode ==
                        currentBranchCode)
                    .Select(x =>
                        x.Visit)
                    .ToList();

            var actualVisits =
                branchVisits.Count;

            var positiveVisits =
                branchVisits.Count(
                    IsPositiveVisit);

            var negativeVisits =
                actualVisits -
                positiveVisits;

            var customersVisited =
                branchVisits
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.CustomerCode))
                    .Select(x =>
                        NormalizeCode(
                            x.CustomerCode))
                    .Distinct()
                    .Count();

            var activeSalesReps =
                branchVisits
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.SalesRepCode))
                    .Select(x =>
                        NormalizeCode(
                            x.SalesRepCode))
                    .Distinct()
                    .Count();

            var monthlyTarget =
                target?.MonthlyTarget
                ?? 0;

            var plannedVisits =
                target?.PlannedVisits
                ?? 0;

            var achievementPercentage =
                monthlyTarget <= 0
                    ? 0
                    : Math.Round(
                        actualSales /
                        monthlyTarget *
                        100,
                        2);

            var remainingToTarget =
                Math.Max(
                    0,
                    monthlyTarget -
                    actualSales);

            var paceGap =
                Math.Round(
                    achievementPercentage -
                    expectedAchievementPercentage,
                    2);

            // =====================================================
            // Required Daily Sales
            // =====================================================

            decimal requiredDailySales;

            if (remainingToTarget <= 0)
            {
                requiredDailySales =
                    0;
            }
            else if (remainingDays <= 0)
            {
                requiredDailySales =
                    remainingToTarget;
            }
            else
            {
                requiredDailySales =
                    Math.Round(
                        remainingToTarget /
                        remainingDays,
                        2);
            }

            // =====================================================
            // Visits
            // =====================================================

            var visitAchievementPercentage =
                plannedVisits <= 0
                    ? 0
                    : Math.Round(
                        (decimal)actualVisits /
                        plannedVisits *
                        100,
                        2);

            var positiveVisitPercentage =
                CalculatePercentage(
                    positiveVisits,
                    actualVisits);

            // =====================================================
            // Operational Data Availability
            //
            // وجود Target فقط لا يعني أن لدينا Actual Data.
            // لذلك لا نحكم على أداء الفرع قبل رفع بياناته.
            // =====================================================

            var hasOperationalData =
                actualSales != 0 ||
                actualVisits > 0;

            // =====================================================
            // Performance Status
            // =====================================================

            string performanceStatus;

            if (monthlyTarget <= 0)
            {
                performanceStatus =
                    "بدون تارجت";
            }
            else if (!hasOperationalData)
            {
                performanceStatus =
                    "لا توجد بيانات تشغيلية";
            }
            else if (achievementPercentage >= 100)
            {
                performanceStatus =
                    "محقق التارجت";
            }
            else if (paceGap >= 5)
            {
                performanceStatus =
                    "متقدم عن المعدل";
            }
            else if (paceGap >= -5)
            {
                performanceStatus =
                    "يسير حسب المعدل";
            }
            else if (paceGap >= -15)
            {
                performanceStatus =
                    "متأخر";
            }
            else
            {
                performanceStatus =
                    "متأخر بشكل ملحوظ";
            }

            // =====================================================
            // Branch Name
            // =====================================================

            var branchName =
                target?.BranchName;

            if (string.IsNullOrWhiteSpace(
                branchName))
            {
                branchName =
                    customers
                        .FirstOrDefault(x =>
                            NormalizeCode(
                                x.BranchCode)
                            ==
                            currentBranchCode)
                        ?.BranchName
                    ?? currentBranchCode;
            }

            // =====================================================
            // Result
            // =====================================================

            result.Add(
                new BranchPerformanceAnalysisDto
                {
                    BranchCode =
                        currentBranchCode,

                    BranchName =
                        branchName,

                    MonthlyTarget =
                        monthlyTarget,

                    ActualSales =
                        actualSales,

                    AchievementPercentage =
                        achievementPercentage,

                    RemainingToTarget =
                        remainingToTarget,

                    ExpectedAchievementPercentage =
                        expectedAchievementPercentage,

                    PaceGapPercentage =
                        paceGap,

                    RequiredDailySales =
                        requiredDailySales,

                    PlannedVisits =
                        plannedVisits,

                    ActualVisits =
                        actualVisits,

                    PositiveVisits =
                        positiveVisits,

                    NegativeVisits =
                        negativeVisits,

                    VisitAchievementPercentage =
                        visitAchievementPercentage,

                    PositiveVisitPercentage =
                        positiveVisitPercentage,

                    ActiveSalesReps =
                        activeSalesReps,

                    CustomersVisited =
                        customersVisited,

                    PerformanceStatus =
                        performanceStatus
                });
        }

        // =========================================================
        // Ranking
        //
        // الفروع التي لم تُرفع لها بيانات تشغيلية
        // تظهر في التقرير ولكن لا تدخل في ترتيب الأداء.
        // =========================================================

        result =
            result
                .OrderBy(x =>
                    x.PerformanceStatus ==
                    "لا توجد بيانات تشغيلية"
                        ? 1
                        : 0)
                .ThenByDescending(x =>
                    x.PaceGapPercentage)
                .ThenByDescending(x =>
                    x.AchievementPercentage)
                .ThenByDescending(x =>
                    x.ActualSales)
                .ToList();

        var rank = 1;

        foreach (var branch in result)
        {
            if (branch.PerformanceStatus ==
                "لا توجد بيانات تشغيلية")
            {
                branch.Rank = 0;
                continue;
            }

            branch.Rank =
                rank++;

        }

        return result;
    }



    private static List<DateRangeSelection> SelectNonOverlappingRanges(
    List<DateRangeSelection> ranges)
    {
        if (ranges == null ||
            ranges.Count == 0)
        {
            return new List<DateRangeSelection>();
        }

        // الأوسع والأحدث أولاً.
        //
        // مثال:
        // 01 -> 23
        // 20 -> 23
        //
        // نستخدم 01 -> 23 فقط لمنع التكرار.

        var orderedRanges =
            ranges
                .OrderByDescending(x =>
                    (x.ToDate - x.FromDate).TotalDays)
                .ThenByDescending(x =>
                    x.ToDate)
                .ThenByDescending(x =>
                    x.UploadBatchId)
                .ToList();

        var selected =
            new List<DateRangeSelection>();

        foreach (var range in orderedRanges)
        {
            var overlaps =
                selected.Any(existing =>
                    RangesOverlap(
                        range.FromDate,
                        range.ToDate,
                        existing.FromDate,
                        existing.ToDate));

            if (overlaps)
            {
                continue;
            }

            selected.Add(range);
        }

        return selected
            .OrderBy(x => x.FromDate)
            .ToList();
    }

    private static bool IsDateCoveredByRanges(
        DateTime date,
        List<DateRangeSelection> ranges)
    {
        var normalizedDate =
            date.Date;

        return ranges.Any(x =>
            normalizedDate >= x.FromDate.Date &&
            normalizedDate <= x.ToDate.Date);
    }

    private static bool RangesOverlap(
        DateTime firstFrom,
        DateTime firstTo,
        DateTime secondFrom,
        DateTime secondTo)
    {
        return
            firstFrom.Date <= secondTo.Date &&
            firstTo.Date >= secondFrom.Date;
    }


    private static bool IsDailySalesInBranch(
        SalesAnalyticsDailySalesReportEntity row,
        string selectedBranchCode,
        Dictionary<string, string> customerBranchMap,
        Dictionary<string, string> lineBranchMap)
    {
        var normalizedCustomerCode = NormalizeCode(row.CustomerCode);

        if (!string.IsNullOrWhiteSpace(normalizedCustomerCode) &&
            customerBranchMap.ContainsKey(normalizedCustomerCode))
        {
            return customerBranchMap[normalizedCustomerCode] == selectedBranchCode;
        }

        var normalizedLineCode = NormalizeCode(row.LineCode);

        return !string.IsNullOrWhiteSpace(normalizedLineCode) &&
               lineBranchMap.ContainsKey(normalizedLineCode) &&
               lineBranchMap[normalizedLineCode] == selectedBranchCode;
    }

    private static string ResolveSalesDistrictCode(
        string? customerCode,
        string? fallbackLineCode,
        Dictionary<string, string> customerDistrictMap)
    {
        var normalizedCustomerCode = NormalizeCode(customerCode);

        if (!string.IsNullOrWhiteSpace(normalizedCustomerCode) &&
            customerDistrictMap.ContainsKey(normalizedCustomerCode) &&
            !string.IsNullOrWhiteSpace(customerDistrictMap[normalizedCustomerCode]))
        {
            return customerDistrictMap[normalizedCustomerCode];
        }

        return NormalizeCode(fallbackLineCode);
    }

    private static string NormalizeCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return string.Empty;

        var cleaned = code.Trim();

        return cleaned.TrimStart('0');
    }

    private static bool IsPositiveVisit(dynamic visit)
    {
        var status = visit.VisitStatus?.ToString()?.Trim() ?? string.Empty;

        if (status.Contains("إيجاب") ||
            status.Contains("ايجاب") ||
            status.Contains("Positive", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return visit.SuccessfulVisitValue > 0;
    }

    private static decimal CalculatePercentage(decimal value, decimal total)
    {
        if (total == 0)
            return 0;

        return Math.Round((value / total) * 100, 2);
    }

    private static decimal CalculatePercentage(int value, int total)
    {
        if (total == 0)
            return 0;

        return Math.Round(((decimal)value / total) * 100, 2);
    }

    private sealed class SalesToDateRow
    {
        public string? LineCode { get; set; }
        public string? LineName { get; set; }

        public string? CityCode { get; set; }
        public string? CityName { get; set; }

        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Unit { get; set; }

        public decimal Quantity { get; set; }
        public decimal SalesAmount { get; set; }
    }

    private sealed class VisitToDateRow
    {
        public string? SupervisorName { get; set; }
        public string? CityName { get; set; }
        public string? VisitCode { get; set; }

        public string SalesRepCode { get; set; } = string.Empty;
        public string SalesRepName { get; set; } = string.Empty;

        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

        public string? VisitStatus { get; set; }
        public string? NegativeReason { get; set; }

        public decimal SuccessfulVisitValue { get; set; }

        public DateTime? VisitStartTime { get; set; }
        public DateTime? VisitEndTime { get; set; }

        public string? VisitDurationText { get; set; }
    }


    private sealed class DateRangeSelection
    {
        public int UploadBatchId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }

}
