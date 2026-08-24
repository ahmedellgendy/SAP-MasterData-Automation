namespace MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

public class SalesAnalyticsCustomerEntity
{
    public int Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerAccountGroup { get; set; }

    public string? BranchCode { get; set; }

    public string? BranchName { get; set; }

    public string? SalesDistrictCode { get; set; }

    public string? SalesDistrictName { get; set; }

    public string? CustomerClassificationCode { get; set; }

    public string? CustomerClassificationName { get; set; }

    public string? IncotermsCode { get; set; }

    public string? IncotermsName { get; set; }

    public string? SearchTerm { get; set; }

    public string? SearchTerm2 { get; set; }

    public DateTime? SourceCreatedDate { get; set; }

    public string? SourceCreatedBy { get; set; }

    public DateTime ImportedAt { get; set; } = DateTime.Now;

    public int? UploadBatchId { get; set; }
}