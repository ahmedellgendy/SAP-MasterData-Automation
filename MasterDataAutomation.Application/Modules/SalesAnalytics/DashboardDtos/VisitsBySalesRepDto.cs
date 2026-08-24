namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class VisitsBySalesRepDto
{
    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public int TotalVisits { get; set; }

    public int PositiveVisits { get; set; }

    public int NegativeVisits { get; set; }

    public decimal PositiveVisitPercentage { get; set; }

    public decimal NegativeVisitPercentage { get; set; }

    public decimal TotalVisitValue { get; set; }

    public decimal AverageVisitValue { get; set; }

    // Executive performance score from 0 to 100
    public decimal PerformanceScore { get; set; }

    // ممتاز - جيد - يحتاج متابعة - حرج
    public string PerformanceStatus { get; set; } = string.Empty;

    // ترتيب المندوب
    public int Rank { get; set; }
}