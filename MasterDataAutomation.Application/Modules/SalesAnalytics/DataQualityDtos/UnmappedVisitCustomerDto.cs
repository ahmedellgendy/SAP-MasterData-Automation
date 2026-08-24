namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class UnmappedVisitCustomerDto
{
    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string? SalesRepCode { get; set; }

    public string? SalesRepName { get; set; }

    public int VisitsCount { get; set; }

    public decimal VisitValue { get; set; }
}