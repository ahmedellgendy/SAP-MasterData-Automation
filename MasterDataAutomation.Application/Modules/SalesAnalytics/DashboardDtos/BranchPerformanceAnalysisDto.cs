namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class BranchPerformanceAnalysisDto
{
    public int Rank { get; set; }

    public string BranchCode { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    // =========================================================
    // Sales
    // =========================================================

    public decimal MonthlyTarget { get; set; }

    public decimal ActualSales { get; set; }

    public decimal AchievementPercentage { get; set; }

    public decimal RemainingToTarget { get; set; }

    // Expected achievement based on selected day of month
    public decimal ExpectedAchievementPercentage { get; set; }

    // Actual Achievement - Expected Achievement
    public decimal PaceGapPercentage { get; set; }

    public decimal RequiredDailySales { get; set; }

    // =========================================================
    // Visits
    // =========================================================

    public int PlannedVisits { get; set; }

    public int ActualVisits { get; set; }

    public int PositiveVisits { get; set; }

    public int NegativeVisits { get; set; }

    public decimal VisitAchievementPercentage { get; set; }

    public decimal PositiveVisitPercentage { get; set; }

    // =========================================================
    // Sales Reps
    // =========================================================

    public int ActiveSalesReps { get; set; }

    // =========================================================
    // Customers
    // =========================================================

    public int CustomersVisited { get; set; }

    // =========================================================
    // Status
    // =========================================================

    public string PerformanceStatus { get; set; } = string.Empty;
}