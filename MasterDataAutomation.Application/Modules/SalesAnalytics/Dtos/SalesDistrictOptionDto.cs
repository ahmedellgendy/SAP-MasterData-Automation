namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

public class SalesDistrictOptionDto
{
    public string SalesDistrictCode { get; set; } = string.Empty;

    public string SalesDistrictName { get; set; } = string.Empty;

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }
}