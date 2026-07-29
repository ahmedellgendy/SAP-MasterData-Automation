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

        var salesRows = _context.SalesAnalyticsDailySalesReports
            .AsNoTracking()
            .Where(x => x.ReportDate >= date && x.ReportDate < nextDate)
            .ToList();

        var visitRows = _context.SalesAnalyticsDailyVisitReports
            .AsNoTracking()
            .Where(x => x.ReportDate >= date && x.ReportDate < nextDate)
            .ToList();

        var monthSalesRows = _context.SalesAnalyticsDailySalesReports
            .AsNoTracking()
            .Where(x => x.ReportDate >= monthStart && x.ReportDate < nextDate)
            .ToList();

        var latestMtdToDate = _context.SalesAnalyticsMtdSalesReports
            .AsNoTracking()
            .Where(x =>
                x.Year == date.Year &&
                x.Month == date.Month &&
                x.ToDate <= date)
            .Max(x => (DateTime?)x.ToDate);

        var mtdSalesRows = latestMtdToDate == null
            ? new List<SalesAnalyticsMtdSalesReportEntity>()
            : _context.SalesAnalyticsMtdSalesReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.ToDate == latestMtdToDate.Value)
                .ToList();

        var monthVisitRows = _context.SalesAnalyticsDailyVisitReports
            .AsNoTracking()
            .Where(x => x.ReportDate >= monthStart && x.ReportDate < nextDate)
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
            salesRows = salesRows
                .Where(x =>
                    lineBranchMap.ContainsKey(NormalizeCode(x.LineCode)) &&
                    lineBranchMap[NormalizeCode(x.LineCode)] == selectedBranchCode)
                .ToList();

            monthSalesRows = monthSalesRows
                .Where(x =>
                    lineBranchMap.ContainsKey(NormalizeCode(x.LineCode)) &&
                    lineBranchMap[NormalizeCode(x.LineCode)] == selectedBranchCode)
                .ToList();

            visitRows = visitRows
                .Where(x =>
                    customerBranchMap.ContainsKey(NormalizeCode(x.CustomerCode)) &&
                    customerBranchMap[NormalizeCode(x.CustomerCode)] == selectedBranchCode)
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
        }

        var totalSales = salesRows.Sum(x => x.SalesAmount);
        var totalQuantity = salesRows.Sum(x => x.Quantity);

        var totalVisits = visitRows.Count;
        var positiveVisits = visitRows.Count(IsPositiveVisit);
        var negativeVisits = totalVisits - positiveVisits;

        var totalVisitValue = visitRows.Sum(x => x.SuccessfulVisitValue);

        var salesByLineMonth = mtdSalesRows
               .Select(x => new
               {
                   Sales = x,
                   NormalizedCustomerCode = NormalizeCode(x.CustomerCode)
               })
               .Where(x =>
                   !string.IsNullOrWhiteSpace(x.NormalizedCustomerCode) &&
                   customerDistrictMap.ContainsKey(x.NormalizedCustomerCode))
               .GroupBy(x => NormalizeCode(customerDistrictMap[x.NormalizedCustomerCode]))
               .ToDictionary(
                   g => g.Key,
                   g => g.Sum(x => x.Sales.TotalAfterTax > 0
                       ? x.Sales.TotalAfterTax
                       : x.Sales.SalesAmount));

        var visitsByLineMonth = monthVisitRows
            .Select(x => new
            {
                NormalizedCustomerCode = NormalizeCode(x.CustomerCode)
            })
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.NormalizedCustomerCode) &&
                customerDistrictMap.ContainsKey(x.NormalizedCustomerCode))
            .GroupBy(x => NormalizeCode(customerDistrictMap[x.NormalizedCustomerCode]))
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
        var totalActualSalesToDate = allTargetAchievements.Sum(x => x.ActualSales);
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

        var alerts = new List<CeoAlertDto>();

        if (targetSummary.TotalMonthlyTarget > 0 && targetSummary.AchievementPercentage < 50)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "Low Target Achievement",
                Message = $"Target achievement is only {targetSummary.AchievementPercentage:N2}%. Required daily sales: {targetSummary.RequiredDailySales:N0} EGP.",
                Severity = "Critical",
                Icon = "bi-exclamation-octagon"
            });
        }

        if (targetSummary.CriticalLinesCount > 0)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "Critical Sales Lines",
                Message = $"{targetSummary.CriticalLinesCount} sales lines are below 50% target achievement.",
                Severity = "Critical",
                Icon = "bi-bullseye"
            });
        }

        if (targetSummary.PlannedVisits > 0 && targetSummary.VisitAchievementPercentage < 70)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "Low Visit Achievement",
                Message = $"Visit achievement is only {targetSummary.VisitAchievementPercentage:N2}% based on uploaded visits data.",
                Severity = "Warning",
                Icon = "bi-geo-alt"
            });
        }

        if (negativeVisits > 0)
        {
            var negativeVisitPercentage = CalculatePercentage(negativeVisits, totalVisits);

            alerts.Add(new CeoAlertDto
            {
                Title = "Negative Visits Recorded",
                Message = $"{negativeVisits} negative visits recorded today, representing {negativeVisitPercentage:N2}% of total visits.",
                Severity = negativeVisitPercentage >= 25 ? "Warning" : "Info",
                Icon = "bi-exclamation-triangle"
            });
        }

        var topNegativeReason = visitRows
            .Where(x => !IsPositiveVisit(x))
            .GroupBy(x => string.IsNullOrWhiteSpace(x.NegativeReason)
                ? "No Reason"
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
                Title = "Top Negative Reason",
                Message = $"Most repeated negative reason: {topNegativeReason.Reason} ({topNegativeReason.Count} times).",
                Severity = "Info",
                Icon = "bi-chat-left-text"
            });
        }

        if (totalSales == 0 && totalVisits == 0)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "No Data Found",
                Message = "No uploaded sales or visits data found for the selected date and branch.",
                Severity = "Warning",
                Icon = "bi-database-exclamation"
            });
        }

        var dashboard = new SalesAnalyticsDashboardDto
        {
            ReportDate = date,

            Kpis = new SalesAnalyticsKpiDto
            {
                TotalSales = totalSales,
                TotalQuantity = totalQuantity,
                TotalVisits = totalVisits,
                PositiveVisits = positiveVisits,
                NegativeVisits = negativeVisits,
                PositiveVisitPercentage = CalculatePercentage(positiveVisits, totalVisits),
                ActiveSalesReps = visitRows
                    .Where(x => !string.IsNullOrWhiteSpace(x.SalesRepCode))
                    .Select(x => x.SalesRepCode)
                    .Distinct()
                    .Count(),
                AverageVisitValue = positiveVisits == 0
                    ? 0
                    : Math.Round(totalVisitValue / positiveVisits, 2)
            },

            TargetSummary = targetSummary,
            Alerts = alerts,
            TargetAchievements = lowestTargetAchievements,
            LowestTargetAchievements = lowestTargetAchievements,
            BestTargetAchievements = bestTargetAchievements,

            SalesByLines = salesRows
                .GroupBy(x => new { x.LineCode, x.LineName })
                .Select(g => new SalesByLineDto
                {
                    LineCode = g.Key.LineCode,
                    LineName = g.Key.LineName,
                    TotalSales = g.Sum(x => x.SalesAmount),
                    TotalQuantity = g.Sum(x => x.Quantity),
                    ContributionPercentage = CalculatePercentage(g.Sum(x => x.SalesAmount), totalSales)
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(10)
                .ToList(),

            VisitsBySalesReps = visitRows
                .GroupBy(x => new { x.SalesRepCode, x.SalesRepName })
                .Select(g =>
                {
                    var repTotalVisits = g.Count();
                    var repPositiveVisits = g.Count(IsPositiveVisit);
                    var repNegativeVisits = repTotalVisits - repPositiveVisits;

                    return new VisitsBySalesRepDto
                    {
                        SalesRepCode = g.Key.SalesRepCode,
                        SalesRepName = g.Key.SalesRepName,
                        TotalVisits = repTotalVisits,
                        PositiveVisits = repPositiveVisits,
                        NegativeVisits = repNegativeVisits,
                        PositiveVisitPercentage = CalculatePercentage(repPositiveVisits, repTotalVisits),
                        TotalVisitValue = g.Sum(x => x.SuccessfulVisitValue)
                    };
                })
                .OrderByDescending(x => x.TotalVisits)
                .Take(10)
                .ToList(),

            BottomCustomersToday = visitRows
                .GroupBy(x => new
                {
                    x.CustomerCode,
                    x.CustomerName,
                    x.SalesRepCode,
                    x.SalesRepName,
                    x.VisitStatus,
                    x.NegativeReason
                })
                .Select(g => new BottomCustomerTodayDto
                {
                    CustomerCode = g.Key.CustomerCode,
                    CustomerName = g.Key.CustomerName,
                    SalesRepCode = g.Key.SalesRepCode,
                    SalesRepName = g.Key.SalesRepName,
                    VisitStatus = g.Key.VisitStatus,
                    NegativeReason = g.Key.NegativeReason,
                    SalesValue = g.Sum(x => x.SuccessfulVisitValue)
                })
                .OrderBy(x => x.SalesValue)
                .ThenBy(x => x.CustomerName)
                .Take(5)
                .ToList(),

            NegativeVisitReasons = visitRows
                .Where(x => !IsPositiveVisit(x))
                .GroupBy(x => string.IsNullOrWhiteSpace(x.NegativeReason)
                    ? "No Reason"
                    : x.NegativeReason.Trim())
                .Select(g => new NegativeVisitReasonDto
                {
                    Reason = g.Key,
                    Count = g.Count(),
                    Percentage = CalculatePercentage(g.Count(), negativeVisits)
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList(),

            TopProducts = salesRows
                .GroupBy(x => x.ProductName)
                .Select(g => new ProductPerformanceDto
                {
                    ProductName = g.Key,
                    TotalSales = g.Sum(x => x.SalesAmount),
                    TotalQuantity = g.Sum(x => x.Quantity),
                    ContributionPercentage = CalculatePercentage(g.Sum(x => x.SalesAmount), totalSales)
                })
                .OrderByDescending(x => x.TotalSales)
                .Take(10)
                .ToList()
        };

        return dashboard;
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

        if (latestSalesDate == null && latestVisitsDate == null)
            return null;

        if (latestSalesDate == null)
            return latestVisitsDate.Value.Date;

        if (latestVisitsDate == null)
            return latestSalesDate.Value.Date;

        return latestSalesDate > latestVisitsDate
            ? latestSalesDate.Value.Date
            : latestVisitsDate.Value.Date;
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
}