namespace MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

public class SalesAnalyticsSalesRepEntity
{
    public int Id { get; set; }

    public string SalesRepCode { get; set; } = string.Empty;

    public string SalesRepName { get; set; } = string.Empty;

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public string? RegionCode { get; set; }

    public string? RegionName { get; set; }

    public string? InternalCode { get; set; }

    public string? SearchTerm2 { get; set; }

    public DateTime? SourceCreatedDate { get; set; }

    public string? SourceCreatedBy { get; set; }

    public DateTime ImportedAt { get; set; } = DateTime.Now;

    public int? UploadBatchId { get; set; }
}