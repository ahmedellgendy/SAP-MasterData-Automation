using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareTicketDetailsDto
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? LastActivityAt { get; set; }

    // Customer
    public int? CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerPhone { get; set; }

    public string? CustomerAddress { get; set; }

    // Classification
    public CustomerCareTicketSource Source { get; set; }

    public CustomerCareTicketType Type { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int? SubCategoryId { get; set; }

    public string? SubCategoryName { get; set; }

    public string Description { get; set; } = string.Empty;

    // Workflow
    public CustomerCareTicketPriority Priority { get; set; }

    public CustomerCareTicketStatus Status { get; set; }

    public bool IsCritical { get; set; }

    // Assignment
    public int? AssignedToUserId { get; set; }

    public int? AssignedDepartmentId { get; set; }

    public string? AssignedDepartmentName { get; set; }

    public DateTime? AssignedAt { get; set; }

    // SLA
    public DateTime? FirstResponseDueAt { get; set; }

    public DateTime? FirstRespondedAt { get; set; }

    public DateTime? ResolutionDueAt { get; set; }

    public DateTime? EscalationDueAt { get; set; }

    public bool IsFirstResponseBreached { get; set; }

    public bool IsResolutionBreached { get; set; }

    // Follow-up
    public DateTime? NextFollowUpAt { get; set; }

    // Resolution
    public string? ResolutionSummary { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public int ReopenCount { get; set; }

    // Timeline
    public List<CustomerCareTicketActionDto> Actions { get; set; }
        = new();


    public CustomerCareQualityDetailDto? QualityDetail { get; set; }

    public List<CustomerCareTicketListItemDto> CustomerHistory { get; set; }
        = new();

    public int CreatedByUserId { get; set; }

    public List<CustomerCareTicketGiftDto> Gifts { get; set; }
    = new();

    public int TotalGiftQuantity =>
        Gifts.Sum(x => x.Quantity);

    public List<CustomerCareAttachmentDto> Attachments { get; set; }
    = new();
}