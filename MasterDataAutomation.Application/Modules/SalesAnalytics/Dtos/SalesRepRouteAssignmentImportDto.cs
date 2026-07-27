namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

public class SalesRepRouteAssignmentImportDto
{
    public string SalesRepCode { get; set; } = string.Empty;
    public string SalesRepName { get; set; } = string.Empty;

    public string SalesDistrictCode { get; set; } = string.Empty;
    public string SalesDistrictName { get; set; } = string.Empty;

    public string? BranchCode { get; set; }
    public string? BranchName { get; set; }

    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;
}