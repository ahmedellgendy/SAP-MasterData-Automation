namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

public class SalesAnalyticsDailyVisitImportDto
{
    public DateTime ReportDate { get; set; }

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
}