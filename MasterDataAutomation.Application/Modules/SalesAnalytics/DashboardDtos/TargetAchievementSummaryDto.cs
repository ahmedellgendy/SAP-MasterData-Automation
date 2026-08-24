namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class TargetAchievementSummaryDto
{
    public decimal TotalMonthlyTarget { get; set; }

    public decimal ActualSalesToDate { get; set; }

    public decimal AchievementPercentage { get; set; }

    public decimal RemainingTarget { get; set; }

    public decimal RequiredDailySales { get; set; }

    public int PlannedVisits { get; set; }

    public int ActualVisits { get; set; }

    public decimal VisitAchievementPercentage { get; set; }

    public int CriticalLinesCount { get; set; }

    public int OnTrackLinesCount { get; set; }

    public int AchievedLinesCount { get; set; }
}