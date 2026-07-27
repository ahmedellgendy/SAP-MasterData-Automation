namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

public class SalesAnalyticsDailySalesImportDto
{
    public DateTime ReportDate { get; set; }

    public string LineCode { get; set; } = string.Empty;

    public string LineName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal SalesAmount { get; set; }
}