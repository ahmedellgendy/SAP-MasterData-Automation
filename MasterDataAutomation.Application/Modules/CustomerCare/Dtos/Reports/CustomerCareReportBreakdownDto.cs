
namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos.Reports;

public class CustomerCareReportBreakdownDto
{
    // ID of the branch, category or department.
    // Null means unspecified or not applicable.
    public int? Id { get; set; }

    // Display name.
    public string Name { get; set; } = string.Empty;

    // Total tickets in this group.
    public int TotalTickets { get; set; }

    // Current workload.
    public int OpenTickets { get; set; }

    // Resolved or closed tickets.
    public int CompletedTickets { get; set; }

    // Tickets exceeding resolution SLA.
    public int OverdueTickets { get; set; }

    // Completed tickets / total tickets * 100.
    public decimal ResolutionRate { get; set; }
}
