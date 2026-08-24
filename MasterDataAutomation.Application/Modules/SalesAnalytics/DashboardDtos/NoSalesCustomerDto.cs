namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class NoSalesCustomerDto
{
    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string BranchCode { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public string SalesDistrictCode { get; set; } = string.Empty;

    public string SalesDistrictName { get; set; } = string.Empty;

    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public int TotalVisits { get; set; }

    public int PositiveVisits { get; set; }

    public int NegativeVisits { get; set; }

    public decimal PositiveVisitPercentage { get; set; }

    public decimal SalesValue { get; set; }

    public string? TopNegativeReason { get; set; }

    public string RiskLevel { get; set; } = string.Empty;
}