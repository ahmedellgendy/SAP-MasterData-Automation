namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class SalesByLineDto
{
    public string LineCode { get; set; } = string.Empty;

    public string LineName { get; set; } = string.Empty;

    public decimal TotalSales { get; set; }

    public decimal TotalQuantity { get; set; }

    public decimal ContributionPercentage { get; set; }
}