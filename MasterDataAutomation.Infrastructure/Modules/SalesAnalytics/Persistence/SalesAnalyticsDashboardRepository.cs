using MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Persistence;

public class SalesAnalyticsDashboardRepository : ISalesAnalyticsDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public SalesAnalyticsDashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public SalesAnalyticsDashboardDto GetDashboard(DateTime reportDate)
    {
        var date = reportDate.Date;
        var nextDate = date.AddDays(1);

        var salesRows = _context.SalesAnalyticsDailySalesReports
            .AsNoTracking()
            .Where(x => x.ReportDate >= date && x.ReportDate < nextDate)
            .ToList();

        var visitRows = _context.SalesAnalyticsDailyVisitReports
            .AsNoTracking()
            .Where(x => x.ReportDate >= date && x.ReportDate < nextDate)
            .ToList();

        var totalSales = salesRows.Sum(x => x.SalesAmount);
        var totalQuantity = salesRows.Sum(x => x.Quantity);

        var totalVisits = visitRows.Count;
        var positiveVisits = visitRows.Count(IsPositiveVisit);
        var negativeVisits = totalVisits - positiveVisits;

        var totalVisitValue = visitRows.Sum(x => x.SuccessfulVisitValue);

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
}