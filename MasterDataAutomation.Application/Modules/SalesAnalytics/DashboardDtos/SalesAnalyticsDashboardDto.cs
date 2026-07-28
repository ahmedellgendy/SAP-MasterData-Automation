namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class SalesAnalyticsDashboardDto
{
    public DateTime ReportDate { get; set; }

    public SalesAnalyticsKpiDto Kpis { get; set; } = new();

    public List<SalesByLineDto> SalesByLines { get; set; } = new();

    public List<VisitsBySalesRepDto> VisitsBySalesReps { get; set; } = new();

    public List<BottomCustomerTodayDto> BottomCustomersToday { get; set; } = new();

    public List<NegativeVisitReasonDto> NegativeVisitReasons { get; set; } = new();

    public List<ProductPerformanceDto> TopProducts { get; set; } = new();

    public List<TargetAchievementDto> TargetAchievements { get; set; } = new();
}