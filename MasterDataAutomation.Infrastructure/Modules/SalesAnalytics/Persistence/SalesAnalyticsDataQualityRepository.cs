using MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;
using MasterDataAutomation.Application.Modules.SalesAnalytics.Interfaces;
using MasterDataAutomation.Infrastructure.Data;
using MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;
using Microsoft.EntityFrameworkCore;

namespace MasterDataAutomation.Infrastructure.Modules.SalesAnalytics.Persistence;

public class SalesAnalyticsDataQualityRepository : ISalesAnalyticsDataQualityRepository
{
    private readonly ApplicationDbContext _context;

    public SalesAnalyticsDataQualityRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public SalesAnalyticsDataQualityReportDto GetReport(
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

        // =========================================================
        // Customer Master
        // =========================================================

        var customers =
            _context.SalesAnalyticsCustomers
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

        var customerMasterMap =
            customers
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode))
                .GroupBy(x =>
                    NormalizeCode(
                        x.CustomerCode))
                .ToDictionary(
                    g => g.Key,
                    g => g.First());

        var customerBranchMap =
            customers
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode))
                .GroupBy(x =>
                    NormalizeCode(
                        x.CustomerCode))
                .ToDictionary(
                    g => g.Key,
                    g => NormalizeCode(
                        g.First().BranchCode));

        var districtBranchMap =
            customers
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.SalesDistrictCode))
                .GroupBy(x =>
                    NormalizeCode(
                        x.SalesDistrictCode))
                .ToDictionary(
                    g => g.Key,
                    g => NormalizeCode(
                        g.First().BranchCode));


        // =========================================================
        // Sales Rep Master
        // =========================================================

        var salesReps =
            _context.SalesAnalyticsSalesReps
                .AsNoTracking()
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.SalesRepCode))
                .Select(x => new
                {
                    x.SalesRepCode,
                    x.SalesRepName,
                    x.BranchCode,
                    x.BranchName
                })
                .ToList();

        var salesRepCodeSet =
            salesReps
                .Select(x =>
                    NormalizeCode(
                        x.SalesRepCode))
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .ToHashSet();

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
                .Select(x =>
                    x.UploadBatchId)
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
                .Select(x =>
                    x.UploadBatchId)
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
        // Monthly Targets
        // =========================================================

        var monthlyTargets =
            _context
                .SalesDistrictMonthlyTargets
                .AsNoTracking()
                .Where(x =>
                    x.Year == date.Year &&
                    x.Month == date.Month)
                .ToList();

        // =========================================================
        // Normalize MTD + Daily Sales Into One Shape
        // =========================================================

        var salesRows =
            mtdSalesRows
                .Select(x => new SalesQualityRow
                {
                    CustomerCode =
                        x.CustomerCode,

                    CustomerName =
                        x.CustomerName,

                    LineCode =
                        x.CityCode,

                    LineName =
                        x.CityName,

                    Quantity =
                        x.Quantity,

                    SalesAmount =
                        x.TotalAfterTax > 0
                            ? x.TotalAfterTax
                            : x.SalesAmount
                })
                .Concat(
                    dailySalesRows
                        .Select(x =>
                            new SalesQualityRow
                            {
                                CustomerCode =
                                    x.CustomerCode,

                                CustomerName =
                                    x.CustomerName,

                                LineCode =
                                    !string.IsNullOrWhiteSpace(
                                        x.LineCode)
                                        ? x.LineCode
                                        : x.CityCode,

                                LineName =
                                    !string.IsNullOrWhiteSpace(
                                        x.LineName)
                                        ? x.LineName
                                        : x.CityName,

                                Quantity =
                                    x.Quantity,

                                SalesAmount =
                                    x.TotalAfterTax > 0
                                        ? x.TotalAfterTax
                                        : x.SalesAmount
                            }))
                .ToList();

        // =========================================================
        // Normalize MTD + Daily Visits Into One Shape
        // =========================================================

        var visitRows =
            mtdVisitRows
                .Select(x =>
                    new VisitQualityRow
                    {
                        CustomerCode =
                            x.CustomerCode,

                        CustomerName =
                            x.CustomerName,

                        SalesRepCode =
                            x.SalesRepCode,

                        SalesRepName =
                            x.SalesRepName,

                        SuccessfulVisitValue =
                            x.SuccessfulVisitValue
                    })
                .Concat(
                    dailyVisitRows
                        .Select(x =>
                            new VisitQualityRow
                            {
                                CustomerCode =
                                    x.CustomerCode,

                                CustomerName =
                                    x.CustomerName,

                                SalesRepCode =
                                    x.SalesRepCode,

                                SalesRepName =
                                    x.SalesRepName,

                                SuccessfulVisitValue =
                                    x.SuccessfulVisitValue
                            }))
                .ToList();

        // =========================================================
        // Branch Filter
        // =========================================================

        string? selectedBranchName = null;

        if (!string.IsNullOrWhiteSpace(
            selectedBranchCode))
        {
            selectedBranchName =
                customers
                    .FirstOrDefault(x =>
                        NormalizeCode(
                            x.BranchCode)
                        ==
                        selectedBranchCode)
                    ?.BranchName;

            customers =
                customers
                    .Where(x =>
                        NormalizeCode(
                            x.BranchCode)
                        ==
                        selectedBranchCode)
                    .ToList();

            salesRows =
                salesRows
                    .Where(x =>
                    {
                        var customerCode =
                            NormalizeCode(
                                x.CustomerCode);

                        if (customerMasterMap.TryGetValue(
                            customerCode,
                            out var customer))
                        {
                            return NormalizeCode(
                                       customer.BranchCode)
                                   ==
                                   selectedBranchCode;
                        }

                        var lineCode =
                            NormalizeCode(
                                x.LineCode);

                        return
                            districtBranchMap.TryGetValue(
                                lineCode,
                                out var mappedBranch)
                            &&
                            mappedBranch ==
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
                            customerBranchMap.TryGetValue(
                                customerCode,
                                out var mappedBranch)
                            &&
                            mappedBranch ==
                            selectedBranchCode;
                    })
                    .ToList();

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
        // Sets
        // =========================================================

        var customerCodeSet =
            customers
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode))
                .Select(x =>
                    NormalizeCode(
                        x.CustomerCode))
                .ToHashSet();

        var salesDistrictCodeSet =
            customers
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.SalesDistrictCode))
                .Select(x =>
                    NormalizeCode(
                        x.SalesDistrictCode))
                .ToHashSet();

        // =========================================================
        // Unmapped Visit Customers
        // =========================================================

        var unmappedVisitCustomerGroups =
            visitRows
                .Select(x => new
                {
                    Visit = x,

                    CustomerCode =
                        NormalizeCode(
                            x.CustomerCode)
                })
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode)
                    &&
                    !customerCodeSet.Contains(
                        x.CustomerCode))
                .GroupBy(x => new
                {
                    x.Visit.CustomerCode,
                    x.Visit.CustomerName,
                    x.Visit.SalesRepCode,
                    x.Visit.SalesRepName
                })
                .Select(g =>
                    new UnmappedVisitCustomerDto
                    {
                        CustomerCode =
                            g.Key.CustomerCode,

                        CustomerName =
                            g.Key.CustomerName,

                        SalesRepCode =
                            g.Key.SalesRepCode,

                        SalesRepName =
                            g.Key.SalesRepName,

                        VisitsCount =
                            g.Count(),

                        VisitValue =
                            g.Sum(x =>
                                x.Visit
                                    .SuccessfulVisitValue)
                    })
                .OrderByDescending(x =>
                    x.VisitsCount)
                .ThenByDescending(x =>
                    x.VisitValue)
                .ToList();

        var unmappedVisitCustomersCount =
            unmappedVisitCustomerGroups.Count;

        var unmappedVisitCustomers =
            unmappedVisitCustomerGroups
                .Take(50)
                .ToList();

        // =========================================================
        // Unmapped Sales Customers
        // =========================================================

        var unmappedSalesCustomerGroups =
            salesRows
                .Select(x => new
                {
                    Sales = x,

                    CustomerCode =
                        NormalizeCode(
                            x.CustomerCode)
                })
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.CustomerCode)
                    &&
                    !customerCodeSet.Contains(
                        x.CustomerCode))
                .GroupBy(x => new
                {
                    x.Sales.CustomerCode,
                    x.Sales.CustomerName
                })
                .Select(g =>
                    new UnmappedSalesCustomerDto
                    {
                        CustomerCode =
                            g.Key.CustomerCode,

                        CustomerName =
                            g.Key.CustomerName,

                        RowsCount =
                            g.Count(),

                        TotalSales =
                            g.Sum(x =>
                                x.Sales.SalesAmount),

                        TotalQuantity =
                            g.Sum(x =>
                                x.Sales.Quantity)
                    })
                .OrderByDescending(x =>
                    x.TotalSales)
                .ToList();

        var unmappedSalesCustomersCount =
            unmappedSalesCustomerGroups.Count;

        var unmappedSalesCustomers =
            unmappedSalesCustomerGroups
                .Take(50)
                .ToList();

        // =========================================================
        // Unmapped Sales Lines
        // =========================================================

        var unmappedSalesLineGroups =
            salesRows
                .Select(x => new
                {
                    Sales = x,

                    LineCode =
                        NormalizeCode(
                            x.LineCode)
                })
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.LineCode)
                    &&
                    !salesDistrictCodeSet.Contains(
                        x.LineCode))
                .GroupBy(x => new
                {
                    x.Sales.LineCode,
                    x.Sales.LineName
                })
                .Select(g =>
                    new UnmappedSalesLineDto
                    {
                        LineCode =
                            g.Key.LineCode,

                        LineName =
                            g.Key.LineName,

                        RowsCount =
                            g.Count(),

                        TotalSales =
                            g.Sum(x =>
                                x.Sales.SalesAmount),

                        TotalQuantity =
                            g.Sum(x =>
                                x.Sales.Quantity)
                    })
                .OrderByDescending(x =>
                    x.TotalSales)
                .ToList();

        var unmappedSalesLinesCount =
            unmappedSalesLineGroups.Count;

        var unmappedSalesLines =
            unmappedSalesLineGroups
                .Take(50)
                .ToList();

        // =========================================================
        // Customers Missing Sales District
        // =========================================================

        var customersMissingDistrictAll =
            customers
                .Where(x =>
                    string.IsNullOrWhiteSpace(
                        x.SalesDistrictCode))
                .Select(x =>
                    new CustomerMissingDistrictDto
                    {
                        CustomerCode =
                            x.CustomerCode,

                        CustomerName =
                            x.CustomerName,

                        BranchCode =
                            x.BranchCode,

                        BranchName =
                            x.BranchName
                    })
                .OrderBy(x =>
                    x.BranchName)
                .ThenBy(x =>
                    x.CustomerName)
                .ToList();

        var customersMissingSalesDistrictCount =
            customersMissingDistrictAll.Count;

        var customersMissingSalesDistrict =
            customersMissingDistrictAll
                .Take(50)
                .ToList();

        // =========================================================
        // Sales By District
        // =========================================================

        var salesByLine =
            salesRows
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.LineCode))
                .GroupBy(x =>
                    NormalizeCode(
                        x.LineCode))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(x =>
                        x.SalesAmount));

        // =========================================================
        // Targets Without Sales
        // =========================================================

        var targetsWithoutSalesAll =
            monthlyTargets
                .Where(x =>
                {
                    var districtCode =
                        NormalizeCode(
                            x.SalesDistrictCode);

                    return
                        !salesByLine.TryGetValue(
                            districtCode,
                            out var sales)
                        ||
                        sales <= 0;
                })
                .Select(x =>
                    new TargetWithoutSalesDto
                    {
                        BranchCode =
                            x.BranchCode,

                        BranchName =
                            x.BranchName,

                        SalesDistrictCode =
                            x.SalesDistrictCode,

                        SalesDistrictName =
                            x.SalesDistrictName,

                        MonthlyTarget =
                            x.MonthlySalesTarget,

                        PlannedVisits =
                            x.PlannedVisits
                    })
                .OrderByDescending(x =>
                    x.MonthlyTarget)
                .ToList();

        var targetsWithoutSalesCount =
            targetsWithoutSalesAll.Count;

        var targetsWithoutSales =
            targetsWithoutSalesAll
                .Take(50)
                .ToList();

        // =========================================================
        // Unmapped Sales Reps
        // =========================================================

        var unmappedSalesRepGroups =
            visitRows
                .Select(x => new
                {
                    Visit = x,

                    SalesRepCode =
                        NormalizeCode(
                            x.SalesRepCode)
                })
                .Where(x =>
                    !string.IsNullOrWhiteSpace(
                        x.SalesRepCode)
                    &&
                    !salesRepCodeSet.Contains(
                        x.SalesRepCode))
                .GroupBy(x => new
                {
                    SalesRepCode =
                        NormalizeCode(
                            x.Visit.SalesRepCode),

                    SalesRepName =
                        x.Visit.SalesRepName
                })
                .Select(g =>
                    new UnmappedSalesRepDto
                    {
                        SalesRepCode =
                            g.Key.SalesRepCode,

                        SalesRepName =
                            g.Key.SalesRepName,

                        VisitsCount =
                            g.Count(),

                        CustomersVisited =
                            g.Select(x =>
                                    NormalizeCode(
                                        x.Visit.CustomerCode))
                                .Where(x =>
                                    !string.IsNullOrWhiteSpace(x))
                                .Distinct()
                                .Count(),

                        TotalVisitValue =
                            g.Sum(x =>
                                x.Visit.SuccessfulVisitValue)
                    })
                .OrderByDescending(x =>
                    x.VisitsCount)
                .ThenByDescending(x =>
                    x.TotalVisitValue)
                .ThenBy(x =>
                    x.SalesRepName)
                .ToList();

        var unmappedSalesRepsCount =
            unmappedSalesRepGroups.Count;

        var unmappedSalesReps =
            unmappedSalesRepGroups
                .Take(50)
                .ToList();

        var affectedSalesRepVisitRows =
            unmappedSalesRepGroups
                .Sum(x =>
                    x.VisitsCount);

        // =========================================================
        // Business Impact
        // =========================================================

        var affectedVisitRows =
            unmappedVisitCustomerGroups
                .Sum(x =>
                    x.VisitsCount);

        var affectedVisitValue =
            unmappedVisitCustomerGroups
                .Sum(x =>
                    x.VisitValue);

        var affectedSalesRows =
            unmappedSalesCustomerGroups
                .Sum(x =>
                    x.RowsCount);

        var affectedSalesValue =
            unmappedSalesCustomerGroups
                .Sum(x =>
                    x.TotalSales);

        // =========================================================
        // Issues
        // =========================================================

        var issues = new List<DataQualityIssueDto>();

        if (!salesRows.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "لا توجد بيانات مبيعات",
                Message = "لم يتم العثور على بيانات مبيعات للفترة المحددة من بداية الشهر وحتى التاريخ المختار.",
                Count = 0,
                Severity = "Critical",
                Icon = "bi-file-earmark-excel"
            });
        }

        if (!visitRows.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "لا توجد بيانات زيارات",
                Message = "لم يتم العثور على بيانات زيارات للفترة المحددة من بداية الشهر وحتى التاريخ المختار.",
                Count = 0,
                Severity = "Critical",
                Icon = "bi-geo-alt"
            });
        }

        if (!monthlyTargets.Any())
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "لا توجد بيانات تارجت",
                Message = "لم يتم العثور على تارجت شهري للشهر أو الفرع المحدد.",
                Count = 0,
                Severity = "Warning",
                Icon = "bi-bullseye"
            });
        }

        if (unmappedVisitCustomersCount > 0)
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "عملاء زيارات غير مربوطين",
                Message = "يوجد عملاء في بيانات الزيارات غير موجودين في بيانات العملاء الرئيسية.",
                Count = unmappedVisitCustomersCount,
                Severity = "Critical",
                Icon = "bi-person-x"
            });
        }

        if (unmappedSalesCustomersCount > 0)
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "عملاء مبيعات غير مربوطين",
                Message = "يوجد عملاء في بيانات المبيعات غير موجودين في بيانات العملاء الرئيسية.",
                Count = unmappedSalesCustomersCount,
                Severity = "Critical",
                Icon = "bi-person-exclamation"
            });
        }

        if (unmappedSalesRepsCount > 0)
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "مناديب زيارات غير موجودين في البيانات الرئيسية",
                Message = "يوجد مناديب ظهروا في بيانات الزيارات ولم يتم العثور عليهم داخل Sales Rep Master.",
                Count = unmappedSalesRepsCount,
                Severity = "Critical",
                Icon = "bi-person-badge"
            });
        }

        if (unmappedSalesLinesCount > 0)
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "مناطق بيع غير مربوطة",
                Message = "يوجد أكواد مناطق بيع في بيانات المبيعات لا يمكن ربطها بشكل صحيح ببيانات العملاء.",
                Count = unmappedSalesLinesCount,
                Severity = "Warning",
                Icon = "bi-diagram-3"
            });
        }

        if (customersMissingSalesDistrictCount > 0)
        {
            issues.Add(new DataQualityIssueDto
            {
                Title = "عملاء بدون منطقة بيع",
                Message = "يوجد عملاء في بيانات العملاء الرئيسية بدون كود منطقة بيع.",
                Count = customersMissingSalesDistrictCount,
                Severity = "Warning",
                Icon = "bi-people"
            });
        }
        // =========================================================
        // Severity Counts
        // =========================================================

        var criticalIssuesCount =
            issues.Count(x =>
                x.Severity ==
                "Critical");

        var warningIssuesCount =
            issues.Count(x =>
                x.Severity ==
                "Warning");

        var infoIssuesCount =
            issues.Count(x =>
                x.Severity ==
                "Info");

        // =========================================================
        // Data Quality Score
        //
        // Score is based on affected rows,
        // not raw issue-card count.
        // =========================================================

        var totalOperationalRows =
            salesRows.Count +
            visitRows.Count;

        var affectedOperationalRows =
            affectedSalesRows +
            affectedVisitRows;

        var dataQualityScore =
            totalOperationalRows == 0
                ? 0
                : Math.Round(
                    Math.Max(
                        0,
                        100m -
                        (
                            (decimal)
                            affectedOperationalRows
                            /
                            totalOperationalRows
                            *
                            100m
                        )),
                    2);

        // =========================================================
        // Result
        // =========================================================

        return new SalesAnalyticsDataQualityReportDto
        {
            ReportDate =
                date,

            BranchCode =
                branchCode,

            BranchName =
                selectedBranchName,

            HasSalesData =
                salesRows.Any(),

            HasVisitsData =
                visitRows.Any(),

            HasTargetsData =
                monthlyTargets.Any(),

            TotalSalesRows =
                salesRows.Count,

            TotalVisitRows =
                visitRows.Count,

            TotalTargets =
                monthlyTargets.Count,

            DataQualityScore =
                dataQualityScore,

            CriticalIssuesCount =
                criticalIssuesCount,

            WarningIssuesCount =
                warningIssuesCount,

            InfoIssuesCount =
                infoIssuesCount,

            UnmappedVisitCustomersCount =
                unmappedVisitCustomersCount,

            UnmappedSalesCustomersCount =
                unmappedSalesCustomersCount,

            UnmappedSalesRepsCount =
                unmappedSalesRepsCount,

            AffectedSalesRepVisitRows =
                affectedSalesRepVisitRows,

            UnmappedSalesLinesCount =
                unmappedSalesLinesCount,

            CustomersMissingSalesDistrictCount =
                customersMissingSalesDistrictCount,

            TargetsWithoutSalesCount =
                targetsWithoutSalesCount,

            AffectedVisitRows =
                affectedVisitRows,

            AffectedVisitValue =
                affectedVisitValue,

            AffectedSalesRows =
                affectedSalesRows,

            AffectedSalesValue =
                affectedSalesValue,

            Issues =
                issues,

            UnmappedVisitCustomers =
                unmappedVisitCustomers,

            UnmappedSalesCustomers =
                unmappedSalesCustomers,

            UnmappedSalesReps =
                unmappedSalesReps,

            UnmappedSalesLines =
                unmappedSalesLines,

            CustomersMissingSalesDistrict =
                customersMissingSalesDistrict,

            TargetsWithoutSales =
                targetsWithoutSales
        };
    }

    // =========================================================
    // Helpers
    // =========================================================

    private static List<DateRangeSelection> SelectNonOverlappingRanges(
        List<DateRangeSelection> ranges)
    {
        if (ranges == null ||
            ranges.Count == 0)
        {
            return new List<DateRangeSelection>();
        }

        // نختار الـRanges الأوسع أولاً ثم الأحدث.
        // أي Range متداخل مع Range تم اختياره بالفعل يتم تجاهله
        // لمنع Double Counting.

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
            .OrderBy(x =>
                x.FromDate)
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

    private static string NormalizeCode(
        string? code)
    {
        if (string.IsNullOrWhiteSpace(
            code))
        {
            return string.Empty;
        }

        return code
            .Trim()
            .TrimStart('0');
    }

    // =========================================================
    // Internal Models
    // =========================================================

    private sealed class DateRangeSelection
    {
        public int UploadBatchId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }

    private sealed class SalesQualityRow
    {
        public string CustomerCode { get; set; } =
            string.Empty;

        public string CustomerName { get; set; } =
            string.Empty;

        public string? LineCode { get; set; }

        public string? LineName { get; set; }

        public decimal Quantity { get; set; }

        public decimal SalesAmount { get; set; }
    }

    private sealed class VisitQualityRow
    {
        public string CustomerCode { get; set; } =
            string.Empty;

        public string CustomerName { get; set; } =
            string.Empty;

        public string SalesRepCode { get; set; } =
            string.Empty;

        public string SalesRepName { get; set; } =
            string.Empty;

        public decimal SuccessfulVisitValue { get; set; }
    }
}