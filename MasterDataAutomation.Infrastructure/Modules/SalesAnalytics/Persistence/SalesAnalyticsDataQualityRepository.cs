using MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Persistence;

public class SalesAnalyticsDataQualityRepository : ISalesAnalyticsDataQualityRepository
{
    private readonly ApplicationDbContext _context;

    public SalesAnalyticsDataQualityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public SalesAnalyticsDataQualityReportDto GetReport(DateTime reportDate, string? branchCode = null)
    {
        var date = reportDate.Date;
        var nextDate = date.AddDays(1);
        var monthStart = new DateTime(date.Year, date.Month, 1);

        var selectedBranchCode = NormalizeCode(branchCode);

        var customers = _context.SalesAnalyticsCustomers
            .AsNoTracking()
            .Select(x => new
            {
                x.CustomerCode,
                x.CustomerName,
                x.BranchCode,
                x.BranchName,
                x.SalesDistrictCode,
                x.SalesDistrictName
            })
            .ToList();

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

        var monthlyTargets = _context.SalesDistrictMonthlyTargets
            .AsNoTracking()
            .Where(x => x.Year == date.Year && x.Month == date.Month)
            .ToList();

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

        string? selectedBranchName = null;

        if (!string.IsNullOrWhiteSpace(selectedBranchCode))
        {
            selectedBranchName = customers
                .FirstOrDefault(x => NormalizeCode(x.BranchCode) == selectedBranchCode)
                ?.BranchName;

            customers = customers
                .Where(x => NormalizeCode(x.BranchCode) == selectedBranchCode)
                .ToList();

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

            monthlyTargets = monthlyTargets
                .Where(x => NormalizeCode(x.BranchCode) == selectedBranchCode)
                .ToList();
        }

        var customerCodeSet = customers
            .Where(x => !string.IsNullOrWhiteSpace(x.CustomerCode))
            .Select(x => NormalizeCode(x.CustomerCode))
            .ToHashSet();

        var salesDistrictCodeSet = customers
            .Where(x => !string.IsNullOrWhiteSpace(x.SalesDistrictCode))
            .Select(x => NormalizeCode(x.SalesDistrictCode))
            .ToHashSet();

        var unmappedVisitCustomers = visitRows
            .Select(x => new
            {
                Visit = x,
                NormalizedCustomerCode = NormalizeCode(x.CustomerCode)
            })
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.NormalizedCustomerCode) &&
                !customerCodeSet.Contains(x.NormalizedCustomerCode))
            .GroupBy(x => new
            {
                x.Visit.CustomerCode,
                x.Visit.CustomerName,
                x.Visit.SalesRepCode,
                x.Visit.SalesRepName
            })
            .Select(g => new UnmappedVisitCustomerDto
            {
                CustomerCode = g.Key.CustomerCode,
                CustomerName = g.Key.CustomerName,
                SalesRepCode = g.Key.SalesRepCode,
                SalesRepName = g.Key.SalesRepName,
                VisitsCount = g.Count(),
                VisitValue = g.Sum(x => x.Visit.SuccessfulVisitValue)
            })
            .OrderByDescending(x => x.VisitsCount)
            .ThenByDescending(x => x.VisitValue)
            .Take(50)
            .ToList();

        var unmappedSalesLines = salesRows
            .Select(x => new
            {
                Sales = x,
                NormalizedLineCode = NormalizeCode(x.LineCode)
            })
            .Where(x =>
                !string.IsNullOrWhiteSpace(x.NormalizedLineCode) &&
                !salesDistrictCodeSet.Contains(x.NormalizedLineCode))
            .GroupBy(x => new
            {
                x.Sales.LineCode,
                x.Sales.LineName
            })
            .Select(g => new UnmappedSalesLineDto
            {
                LineCode = g.Key.LineCode,
                LineName = g.Key.LineName,
                RowsCount = g.Count(),
                TotalSales = g.Sum(x => x.Sales.SalesAmount),
                TotalQuantity = g.Sum(x => x.Sales.Quantity)
            })
            .OrderByDescending(x => x.TotalSales)
            .Take(50)
            .ToList();

        var customersMissingSalesDistrict = customers
            .Where(x => string.IsNullOrWhiteSpace(x.SalesDistrictCode))
            .Select(x => new CustomerMissingDistrictDto
            {
                CustomerCode = x.CustomerCode,
                CustomerName = x.CustomerName,
                BranchCode = x.BranchCode,
                BranchName = x.BranchName
            })
            .OrderBy(x => x.BranchName)
            .ThenBy(x => x.CustomerName)
            .Take(50)
            .ToList();

        var salesByLineMonth = monthSalesRows
            .GroupBy(x => NormalizeCode(x.LineCode))
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => x.SalesAmount));

        var targetsWithoutSales = monthlyTargets
            .Where(x =>
                !salesByLineMonth.ContainsKey(NormalizeCode(x.SalesDistrictCode)) ||
                salesByLineMonth[NormalizeCode(x.SalesDistrictCode)] == 0)
            .Select(x => new TargetWithoutSalesDto
            {
                BranchCode = x.BranchCode,
                BranchName = x.BranchName,
                SalesDistrictCode = x.SalesDistrictCode,
                SalesDistrictName = x.SalesDistrictName,
                MonthlyTarget = x.MonthlySalesTarget,
                PlannedVisits = x.PlannedVisits
            })
            .OrderByDescending(x => x.MonthlyTarget)
            .Take(50)
            .ToList();

        var issues = new List<DataQualityIssueDto>();

        if (!salesRows.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "No Sales Data",
                Message = "No daily sales report data found for the selected date and branch.",
                Count = 0,
                Severity = "Warning",
                Icon = "bi-file-earmark-excel"
            });
        }

        if (!visitRows.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "No Visits Data",
                Message = "No visits report data found for the selected date and branch.",
                Count = 0,
                Severity = "Warning",
                Icon = "bi-geo-alt"
            });
        }

        if (unmappedVisitCustomers.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "Unmapped Visit Customers",
                Message = "Some customers found in visits report are not found in Customer Master.",
                Count = unmappedVisitCustomers.Count,
                Severity = "Critical",
                Icon = "bi-person-x"
            });
        }

        if (unmappedSalesLines.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "Unmapped Sales Lines",
                Message = "Some sales line codes found in sales report are not linked to Customer Master sales districts.",
                Count = unmappedSalesLines.Count,
                Severity = "Warning",
                Icon = "bi-diagram-3"
            });
        }

        if (customersMissingSalesDistrict.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "Customers Missing Sales District",
                Message = "Some customers in Customer Master do not have Sales District Code.",
                Count = customersMissingSalesDistrict.Count,
                Severity = "Warning",
                Icon = "bi-people"
            });
        }

        if (targetsWithoutSales.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "Targets Without Sales",
                Message = "Some target lines have no uploaded sales from month start to selected date.",
                Count = targetsWithoutSales.Count,
                Severity = "Info",
                Icon = "bi-bullseye"
            });
        }

        return new SalesAnalyticsDataQualityReportDto
        {
            ReportDate = date,
            BranchCode = branchCode,
            BranchName = selectedBranchName,

            HasSalesData = salesRows.Any(),
            HasVisitsData = visitRows.Any(),

            TotalSalesRows = salesRows.Count,
            TotalVisitRows = visitRows.Count,

            UnmappedVisitCustomersCount = unmappedVisitCustomers.Count,
            UnmappedSalesLinesCount = unmappedSalesLines.Count,
            CustomersMissingSalesDistrictCount = customersMissingSalesDistrict.Count,
            TargetsWithoutSalesCount = targetsWithoutSales.Count,

            Issues = issues,
            UnmappedVisitCustomers = unmappedVisitCustomers,
            UnmappedSalesLines = unmappedSalesLines,
            CustomersMissingSalesDistrict = customersMissingSalesDistrict,
            TargetsWithoutSales = targetsWithoutSales
        };
    }

    private static string NormalizeCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return string.Empty;

        return code.Trim().TrimStart('0');
    }
}