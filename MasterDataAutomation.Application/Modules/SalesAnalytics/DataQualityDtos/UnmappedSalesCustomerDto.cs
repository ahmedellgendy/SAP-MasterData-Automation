namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class UnmappedSalesCustomerDto
{
    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public int RowsCount { get; set; }

    public decimal TotalSales { get; set; }

    public decimal TotalQuantity { get; set; }
}