namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class SalesAnalyticsDataQualityReportDto
{
    public DateTime ReportDate { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public bool HasSalesData { get; set; }

    public bool HasVisitsData { get; set; }

    public int TotalSalesRows { get; set; }

    public int TotalVisitRows { get; set; }

    public int UnmappedVisitCustomersCount { get; set; }

    public int UnmappedSalesLinesCount { get; set; }

    public int CustomersMissingSalesDistrictCount { get; set; }

    public int TargetsWithoutSalesCount { get; set; }

    public List<DataQualityIssueDto> Issues { get; set; } = new();

    public List<UnmappedVisitCustomerDto> UnmappedVisitCustomers { get; set; } = new();

    public List<UnmappedSalesLineDto> UnmappedSalesLines { get; set; } = new();

    public List<CustomerMissingDistrictDto> CustomersMissingSalesDistrict { get; set; } = new();

    public List<TargetWithoutSalesDto> TargetsWithoutSales { get; set; } = new();
}