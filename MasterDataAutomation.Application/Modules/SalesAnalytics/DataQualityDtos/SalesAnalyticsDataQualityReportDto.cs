namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class SalesAnalyticsDataQualityReportDto
{
    public DateTime ReportDate { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    // =========================================================
    // Data Availability
    // =========================================================

    public bool HasSalesData { get; set; }

    public bool HasVisitsData { get; set; }

    public bool HasTargetsData { get; set; }

    public int TotalSalesRows { get; set; }

    public int TotalVisitRows { get; set; }

    public int TotalTargets { get; set; }

    // =========================================================
    // Quality Summary
    // =========================================================

    public decimal DataQualityScore { get; set; }

    public int CriticalIssuesCount { get; set; }

    public int WarningIssuesCount { get; set; }

    public int InfoIssuesCount { get; set; }

    // =========================================================
    // Mapping Issues
    // =========================================================

    public int UnmappedVisitCustomersCount { get; set; }

    public int UnmappedSalesCustomersCount { get; set; }

    public int UnmappedSalesLinesCount { get; set; }

    public int CustomersMissingSalesDistrictCount { get; set; }

    public int TargetsWithoutSalesCount { get; set; }

    // =========================================================
    // Business Impact
    // =========================================================

    public int AffectedVisitRows { get; set; }

    public decimal AffectedVisitValue { get; set; }

    public int AffectedSalesRows { get; set; }

    public decimal AffectedSalesValue { get; set; }

    // =========================================================
    // Issues
    // =========================================================

    public List<DataQualityIssueDto> Issues { get; set; } = new();

    // =========================================================
    // Details
    // =========================================================

    public List<UnmappedVisitCustomerDto> UnmappedVisitCustomers { get; set; } = new();

    public List<UnmappedSalesCustomerDto> UnmappedSalesCustomers { get; set; } = new();

    public List<UnmappedSalesLineDto> UnmappedSalesLines { get; set; } = new();

    public List<CustomerMissingDistrictDto> CustomersMissingSalesDistrict { get; set; } = new();

    public List<TargetWithoutSalesDto> TargetsWithoutSales { get; set; } = new();


    //UnmappedSalesRep
    public int UnmappedSalesRepsCount { get; set; }

    public int AffectedSalesRepVisitRows { get; set; }

    public List<UnmappedSalesRepDto> UnmappedSalesReps { get; set; } = new();
}