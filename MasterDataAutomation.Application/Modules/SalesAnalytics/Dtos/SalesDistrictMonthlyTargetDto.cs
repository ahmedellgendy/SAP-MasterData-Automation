namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

public class SalesDistrictMonthlyTargetDto
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
}