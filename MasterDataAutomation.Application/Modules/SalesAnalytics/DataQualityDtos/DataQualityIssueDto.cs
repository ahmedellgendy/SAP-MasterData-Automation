namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DataQualityDtos;

public class DataQualityIssueDto
{
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public int Count { get; set; }

    public string Severity { get; set; } = "Info";

    public string Icon { get; set; } = "bi-info-circle";
}