namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareTicketAction
{
    public int Id { get; set; }

    // =========================================================
    // Ticket
    // =========================================================

    public int TicketId { get; set; }

    public CustomerCareTicket Ticket { get; set; } = null!;

    // =========================================================
    // Action
    // =========================================================

    public string ActionType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsInternalNote { get; set; }

    // =========================================================
    // User
    // =========================================================

    public int CreatedByUserId { get; set; }

    public string CreatedByUserName { get; set; } = string.Empty;

    // =========================================================
    // Follow-up
    // =========================================================

    public DateTime? FollowUpAt { get; set; }

    // =========================================================
    // Audit
    // =========================================================

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}