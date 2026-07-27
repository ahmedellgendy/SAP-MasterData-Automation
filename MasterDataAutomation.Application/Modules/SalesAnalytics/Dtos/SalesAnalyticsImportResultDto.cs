namespace MasterDataAutomation.Application.Modules.SalesAnalytics.Dtos;

public class SalesAnalyticsImportResultDto
{
    public bool Success { get; set; }

    public int TotalRows { get; set; }

    public int ImportedRows { get; set; }

    public int FailedRows { get; set; }

    public string? Message { get; set; }
}