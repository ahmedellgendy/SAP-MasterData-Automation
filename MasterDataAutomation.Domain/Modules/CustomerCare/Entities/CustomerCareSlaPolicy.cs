using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareSlaPolicy
{
    public int Id { get; set; }

    // =========================================================
    // Identity
    // =========================================================

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    // =========================================================
    // Scope
    // =========================================================

    public CustomerCareTicketType? TicketType { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public CustomerCareTicketPriority? Priority { get; set; }

    // =========================================================
    // SLA
    // =========================================================

    public int FirstResponseMinutes { get; set; }

    public int ResolutionMinutes { get; set; }

    public int? EscalationMinutes { get; set; }

    // =========================================================
    // Configuration
    // =========================================================

    public bool UseBusinessHours { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    // =========================================================
    // Audit
    // =========================================================

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}