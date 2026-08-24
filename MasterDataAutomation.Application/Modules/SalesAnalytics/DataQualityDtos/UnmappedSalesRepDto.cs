namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class UnmappedSalesRepDto
{
    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public int VisitsCount { get; set; }

    public int CustomersVisited { get; set; }

    public decimal TotalVisitValue { get; set; }
}