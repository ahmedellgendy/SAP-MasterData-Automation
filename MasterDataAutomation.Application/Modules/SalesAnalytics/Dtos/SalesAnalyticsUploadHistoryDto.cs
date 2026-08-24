using MasterDataAutomation.Application.Modules.SalesAnalytics.Enums;

namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

public class SalesAnalyticsUploadHistoryDto
{
    public int Id { get; set; }

    public SalesAnalyticsUploadFileType FileType { get; set; }

    public SalesAnalyticsUploadStatus Status { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public DateTime UploadDate { get; set; }

    public DateTime? ReportDate { get; set; }

    public int TotalRows { get; set; }

    public int ImportedRows { get; set; }

    public int FailedRows { get; set; }

    public string? UploadedBy { get; set; }

    public string? ErrorMessage { get; set; }
}