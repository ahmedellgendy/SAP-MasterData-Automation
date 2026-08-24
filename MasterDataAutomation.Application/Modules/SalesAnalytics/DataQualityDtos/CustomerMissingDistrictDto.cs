namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class CustomerMissingDistrictDto
{
    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }
}