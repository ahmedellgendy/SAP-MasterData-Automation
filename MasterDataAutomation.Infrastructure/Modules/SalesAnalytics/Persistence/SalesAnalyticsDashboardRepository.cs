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

        var latestMtdVisitsToDate = _context.SalesAnalyticsMtdVisitReports
            .AsNoTracking()
            .Where(x =>
                x.Year == date.Year &&
                x.Month == date.Month &&
                x.ToDate <= date)
            .Max(x => (DateTime?)x.ToDate);

        var mtdVisitRows = latestMtdVisitsToDate == null
            ? new List<SalesAnalyticsMtdVisitReportEntity>()
            : _context.SalesAnalyticsMtdVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month &&
                    x.ToDate == latestMtdVisitsToDate.Value)
                .ToList();

        var dailySalesRowsAfterMtd = latestMtdToDate == null
            ? monthSalesRows
            : _context.SalesAnalyticsDailySalesReports
                .AsNoTracking()
                .Where(x =>
                    x.ReportDate > latestMtdToDate.Value &&
                    x.ReportDate < nextDate)
                .ToList();

        var dailyVisitRowsAfterMtd = latestMtdVisitsToDate == null
            ? monthVisitRows
            : _context.SalesAnalyticsDailyVisitReports
                .AsNoTracking()
                .Where(x =>
                    x.ReportDate > latestMtdVisitsToDate.Value &&
                    x.ReportDate < nextDate)
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

        if (negativeVisitsToDate > 0)
        {
            var negativeVisitPercentage = CalculatePercentage(negativeVisitsToDate, totalVisitsToDate);

            alerts.Add(new CeoAlertDto
            {
                Title = "Negative Visits Recorded",
                Message = $"{negativeVisitsToDate} negative visits recorded up to selected date, representing {negativeVisitPercentage:N2}% of total visits.",
                Severity = negativeVisitPercentage >= 25 ? "Warning" : "Info",
                Icon = "bi-exclamation-triangle"
            });
        }

        var topNegativeReason = visitsToDateRows
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

        if (totalSalesToDate == 0 && totalVisitsToDate == 0)
        {
            alerts.Add(new CeoAlertDto
            {
                Title = "No Data Found",
                Message = "No uploaded sales or visits data found for the selected date and branch.",
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
            .ToList();

        var bottomCustomers = visitsToDateRows
            .GroupBy(x => new
            {
                x.CustomerCode,
                x.CustomerName
            })
            .Select(g =>
            {
                var lastVisit = g.Last();

                return new BottomCustomerTodayDto
                {
                    CustomerCode = g.Key.CustomerCode,
                    CustomerName = g.Key.CustomerName,
                    SalesRepCode = lastVisit.SalesRepCode,
                    SalesRepName = lastVisit.SalesRepName,
                    VisitStatus = lastVisit.VisitStatus,
                    NegativeReason = lastVisit.NegativeReason,
                    SalesValue = g.Sum(x => x.SuccessfulVisitValue)
                };
            })
            .OrderBy(x => x.SalesValue)
            .ThenBy(x => x.CustomerName)
            .Take(5)
            .ToList();

        var negativeVisitReasons = visitsToDateRows
            .Where(x => !IsPositiveVisit(x))
            .GroupBy(x => string.IsNullOrWhiteSpace(x.NegativeReason)
                ? "No Reason"
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
}