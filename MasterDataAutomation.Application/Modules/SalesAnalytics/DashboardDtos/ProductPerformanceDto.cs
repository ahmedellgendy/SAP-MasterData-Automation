namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class ProductPerformanceDto
{
    public string ProductName { get; set; } = string.Empty;

    public decimal TotalSales { get; set; }

    public decimal TotalQuantity { get; set; }

    public decimal ContributionPercentage { get; set; }
}