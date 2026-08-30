namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareAssignmentHistory
{
    public int Id { get; set; }

    // =========================================================
    // Ticket
    // =========================================================

    public int TicketId { get; set; }

    public CustomerCareTicket Ticket { get; set; } = null!;

    // =========================================================
    // Assignment
    // =========================================================

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public int? FromDepartmentId { get; set; }

    public int? ToDepartmentId { get; set; }

    public string? Reason { get; set; }

    // =========================================================
    // Audit
    // =========================================================

    public int ChangedByUserId { get; set; }

    public string ChangedByUserName { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}