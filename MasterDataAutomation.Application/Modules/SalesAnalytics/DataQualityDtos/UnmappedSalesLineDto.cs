namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class UnmappedSalesLineDto
{
    public string LineCode { get; set; } = string.Empty;

    public string LineName { get; set; } = string.Empty;

    public int RowsCount { get; set; }

    public decimal TotalSales { get; set; }

    public decimal TotalQuantity { get; set; }
}