using MasterDataAutomation.Application.Modules.SalesAnalytics.Enums;

namespace MasterDataAutomation.Infrastructure.Data.Entities.SalesAnalytics;

public class SalesAnalyticsUploadBatchEntity
{
    public int Id { get; set; }

    public SalesAnalyticsUploadFileType FileType { get; set; }

    public SalesAnalyticsUploadStatus Status { get; set; } = SalesAnalyticsUploadStatus.Success;

    public string OriginalFileName { get; set; } = string.Empty;

    public DateTime UploadDate { get; set; } = DateTime.Now;

    public DateTime? ReportDate { get; set; }

    public int TotalRows { get; set; }

    public int ImportedRows { get; set; }

    public int FailedRows { get; set; }

    public string? UploadedBy { get; set; }

    public string? ErrorMessage { get; set; }
}