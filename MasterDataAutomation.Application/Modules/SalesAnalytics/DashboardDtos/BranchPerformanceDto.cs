namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class BranchPerformanceDto
{
    public string BranchCode { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public decimal MonthlyTarget { get; set; }

    public decimal ActualSales { get; set; }

    public decimal AchievementPercentage { get; set; }

    public decimal RemainingTarget { get; set; }

    public decimal RequiredDailySales { get; set; }

    public int PlannedVisits { get; set; }

    public int ActualVisits { get; set; }

    public decimal VisitAchievementPercentage { get; set; }

    public int TotalLinesCount { get; set; }

    public int CriticalLinesCount { get; set; }

    public int OnTrackLinesCount { get; set; }

    public int AchievedLinesCount { get; set; }
}