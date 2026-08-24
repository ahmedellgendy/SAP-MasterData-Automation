namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class TargetAchievementDto
{
    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public string SalesDistrictCode { get; set; } = string.Empty;

    public string SalesDistrictName { get; set; } = string.Empty;

    public decimal MonthlyTarget { get; set; }

    public decimal ActualSales { get; set; }

    public decimal AchievementPercentage { get; set; }

    public decimal RemainingTarget { get; set; }

    public decimal RequiredDailySales { get; set; }

    public int PlannedVisits { get; set; }

    public int ActualVisits { get; set; }

    public decimal VisitAchievementPercentage { get; set; }

    public string Status { get; set; } = string.Empty;
}