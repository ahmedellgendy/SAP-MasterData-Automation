namespace MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

public class SalesAnalyticsMtdVisitReportEntity
{
    public int Id { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public DateTime? VisitDate { get; set; }

    public string? SupervisorName { get; set; }

    public string? CityName { get; set; }

    public string? VisitCode { get; set; }

    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string? VisitStatus { get; set; }

    public string? NegativeReason { get; set; }

    public decimal SuccessfulVisitValue { get; set; }

    public DateTime? VisitStartTime { get; set; }

    public DateTime? VisitEndTime { get; set; }

    public string? VisitDurationText { get; set; }

    public DateTime ImportedAt { get; set; } = DateTime.Now;

    public int UploadBatchId { get; set; }
}