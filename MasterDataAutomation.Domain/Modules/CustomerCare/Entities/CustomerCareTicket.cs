using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareTicket
{
    public int Id { get; set; }

    // =========================================================
    // Identity
    // =========================================================

    public string TicketNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastActivityAt { get; set; }
    // =========================================================
    // Customer
    // =========================================================

    public int? CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerPhone { get; set; }

    public string? CustomerAddress { get; set; }

    // =========================================================
    // Classification
    // =========================================================

    public CustomerCareTicketSource Source { get; set; }

    public CustomerCareTicketType Type { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    // =========================================================
    // Complaint / Request Details
    // =========================================================

    public string Description { get; set; } = string.Empty;

    public CustomerCareTicketPriority Priority { get; set; }
        = CustomerCareTicketPriority.Normal;

    public CustomerCareTicketStatus Status { get; set; }
        = CustomerCareTicketStatus.New;

    // =========================================================
    // Assignment
    // =========================================================

    public int? AssignedToUserId { get; set; }

    public int? AssignedDepartmentId { get; set; }

    public CustomerCareDepartment? AssignedDepartment { get; set; }

    public DateTime? AssignedAt { get; set; }

    // =========================================================
    // Commercial Context
    // =========================================================

    public int? BranchId { get; set; }

    public int? SalesRepId { get; set; }

    public int? SupervisorId { get; set; }

    // =========================================================
    // SLA / Follow-up
    // =========================================================

    public int? SlaPolicyId { get; set; }

    // First response SLA
    public DateTime? FirstResponseDueAt { get; set; }

    public DateTime? FirstRespondedAt { get; set; }

    // Resolution SLA
    public DateTime? ResolutionDueAt { get; set; }

    // Escalation
    public DateTime? EscalationDueAt { get; set; }

    public DateTime? EscalatedAt { get; set; }

    // Follow-up
    public DateTime? NextFollowUpAt { get; set; }

    // SLA flags
    public bool IsFirstResponseBreached { get; set; }

    public bool IsResolutionBreached { get; set; }

    // =========================================================
    // Resolution
    // =========================================================

    public string? ResolutionSummary { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public int ReopenCount { get; set; }

    // =========================================================
    // Flags
    // =========================================================

    public bool IsCritical { get; set; }

    public bool IsActive { get; set; } = true;


    public CustomerCareCategory? Category { get; set; }

    public CustomerCareSubCategory? SubCategory { get; set; }

    public ICollection<CustomerCareTicketAction> Actions { get; set; }
    = new List<CustomerCareTicketAction>();

    public ICollection<CustomerCareAssignmentHistory> AssignmentHistory { get; set; }
    = new List<CustomerCareAssignmentHistory>();

    public ICollection<CustomerCareStatusHistory> StatusHistory { get; set; }
        = new List<CustomerCareStatusHistory>();

    public CustomerCareQualityDetail? QualityDetail { get; set; }

    public ICollection<CustomerCareAttachment> Attachments { get; set; }
        = new List<CustomerCareAttachment>();

    public CustomerCareSlaPolicy? SlaPolicy { get; set; }

    public ICollection<CustomerCareTicketGift> Gifts { get; set; }
    = new List<CustomerCareTicketGift>();
}