namespace MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

public class SalesRepRouteAssignmentEntity
{
    public int Id { get; set; }

    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public string SalesDistrictCode { get; set; } = string.Empty;

    public string SalesDistrictName { get; set; } = string.Empty;

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public DateTime ImportedAt { get; set; } = DateTime.Now;

    public int? UploadBatchId { get; set; }
}