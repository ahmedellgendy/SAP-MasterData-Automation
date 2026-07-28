namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class VisitsBySalesRepDto
{
    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public int TotalVisits { get; set; }

    public int PositiveVisits { get; set; }

    public int NegativeVisits { get; set; }

    public decimal PositiveVisitPercentage { get; set; }

    public decimal TotalVisitValue { get; set; }
}