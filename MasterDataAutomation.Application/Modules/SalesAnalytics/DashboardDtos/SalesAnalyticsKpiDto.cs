namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class SalesAnalyticsKpiDto
{
    public decimal TotalSales { get; set; }

    public decimal TotalQuantity { get; set; }

    public int TotalVisits { get; set; }

    public int PositiveVisits { get; set; }

    public int NegativeVisits { get; set; }

    public decimal PositiveVisitPercentage { get; set; }

    public int ActiveSalesReps { get; set; }

    public decimal AverageVisitValue { get; set; }
}