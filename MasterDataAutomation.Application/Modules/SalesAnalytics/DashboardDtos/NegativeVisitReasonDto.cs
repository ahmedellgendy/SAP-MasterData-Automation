namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class NegativeVisitReasonDto
{
    public string Reason { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal Percentage { get; set; }
}