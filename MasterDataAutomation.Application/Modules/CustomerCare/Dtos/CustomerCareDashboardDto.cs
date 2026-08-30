namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareDashboardDto
{
    // KPIs
    public int TotalTickets { get; set; }

    public int OpenTickets { get; set; }

    public int NewTickets { get; set; }

    public int InProgressTickets { get; set; }

    public int OverdueTickets { get; set; }

    public int ResolvedTickets { get; set; }

    public int ClosedTickets { get; set; }

    public int CriticalTickets { get; set; }

    public int ReopenedTickets { get; set; }

    public int FollowUpDueTickets { get; set; }

    // Performance
    public decimal AverageResolutionHours { get; set; }

    public decimal SlaCompliancePercentage { get; set; }

    public decimal FirstResponseCompliancePercentage { get; set; }

    // Breakdowns
    public List<CustomerCareDashboardBreakdownDto> ByCategory { get; set; }
        = new();

    public List<CustomerCareDashboardBreakdownDto> BySource { get; set; }
        = new();

    public List<CustomerCareDashboardBreakdownDto> ByDepartment { get; set; }
        = new();

    // Attention
    public List<CustomerCareTicketListItemDto> CriticalOpenTickets { get; set; }
        = new();

    public List<CustomerCareTicketListItemDto> OverdueItems { get; set; }
        = new();

    public int TotalGiftQuantity { get; set; }

    public int TicketsWithGifts { get; set; }

    public List<CustomerCareDashboardBreakdownDto> TopGiftProducts { get; set; }
        = new();

    public List<CustomerCareDashboardGiftDto> RecentGifts { get; set; }
        = new();
}

public class CustomerCareDashboardBreakdownDto
{
    public string Name { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal Percentage { get; set; }
}

public class CustomerCareDashboardGiftDto
{
    public int TicketId { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string? RecipientName { get; set; }

    public string? RecipientPhone { get; set; }

    public string CreatedByUserName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}