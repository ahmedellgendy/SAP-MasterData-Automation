namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class TargetWithoutSalesDto
{
    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public string SalesDistrictCode { get; set; } = string.Empty;

    public string SalesDistrictName { get; set; } = string.Empty;

    public decimal MonthlyTarget { get; set; }

    public int PlannedVisits { get; set; }
}