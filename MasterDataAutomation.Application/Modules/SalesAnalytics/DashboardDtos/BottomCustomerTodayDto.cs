namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class BottomCustomerTodayDto
{
    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public string? VisitStatus { get; set; }

    public string? NegativeReason { get; set; }

    public decimal SalesValue { get; set; }
}