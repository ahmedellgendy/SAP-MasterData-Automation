namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

public class SalesAnalyticsMtdSalesImportDto
{
    public string? CityCode { get; set; }

    public string? CityName { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string? Unit { get; set; }

    public decimal Quantity { get; set; }

    public decimal SalesAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxPercentage { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalBeforeTax { get; set; }

    public decimal TotalAfterTax { get; set; }
}