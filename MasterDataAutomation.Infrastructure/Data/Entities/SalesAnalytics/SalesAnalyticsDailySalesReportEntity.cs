namespace MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

public class SalesAnalyticsDailySalesReportEntity
{
    public int Id { get; set; }

    public DateTime ReportDate { get; set; }

    public string LineCode { get; set; } = string.Empty;

    public string LineName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal SalesAmount { get; set; }

    public DateTime ImportedAt { get; set; } = DateTime.Now;

    public int UploadBatchId { get; set; }
}