namespace MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

public class SalesAnalyticsDailySalesReportEntity
{
    public int Id { get; set; }

    public DateTime ReportDate { get; set; }

    // Old compatibility
    public string LineCode { get; set; } = string.Empty;
    public string LineName { get; set; } = string.Empty;

    // New Customer / City standard
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

    public DateTime ImportedAt { get; set; } = DateTime.Now;

    public int UploadBatchId { get; set; }
}