namespace MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

public class SalesDistrictMonthlyTargetEntity
{
    public int Id { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public string SalesDistrictCode { get; set; } = string.Empty;

    public string SalesDistrictName { get; set; } = string.Empty;

    public decimal MonthlySalesTarget { get; set; }

    public int PlannedVisits { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}