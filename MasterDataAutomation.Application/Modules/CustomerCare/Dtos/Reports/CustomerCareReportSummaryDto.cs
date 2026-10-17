
namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos.Reports;

public class CustomerCareReportSummaryDto
{
    // Total Tickets
    public int TotalTickets { get; set; }

    // Current Status
    public int NewTickets { get; set; }
    public int AssignedTickets { get; set; }
    public int InProgressTickets { get; set; }
    public int ResolvedTickets { get; set; }
    public int ClosedTickets { get; set; }

    // Operational Metrics
    public int OpenTickets { get; set; }
    public int CompletedTickets { get; set; }
    public int OverdueTickets { get; set; }
    public int CriticalTickets { get; set; }
    public int ReopenedTickets { get; set; }

    // Performance
    public decimal ResolutionRate { get; set; }
    public decimal ClosureRate { get; set; }
    public double AverageResolutionHours { get; set; }

    // SLA
    public int FirstResponseBreachedTickets { get; set; }
    public int ResolutionBreachedTickets { get; set; }
    public decimal FirstResponseComplianceRate { get; set; }
    public decimal ResolutionComplianceRate { get; set; }
}
