namespace MasterDataAutomation.Application.Modules.SalesAnalytics.DashboardDtos;

public class CeoAlertDto
{
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Severity { get; set; } = "Info";

    public string Icon { get; set; } = "bi-info-circle";
}