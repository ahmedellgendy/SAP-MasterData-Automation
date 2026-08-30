using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareStatusHistory
{
    public int Id { get; set; }

    // =========================================================
    // Ticket
    // =========================================================

    public int TicketId { get; set; }

    public CustomerCareTicket Ticket { get; set; } = null!;

    // =========================================================
    // Status Change
    // =========================================================

    public CustomerCareTicketStatus? FromStatus { get; set; }

    public CustomerCareTicketStatus ToStatus { get; set; }

    public string? Reason { get; set; }

    // =========================================================
    // Audit
    // =========================================================

    public int ChangedByUserId { get; set; }

    public string ChangedByUserName { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}