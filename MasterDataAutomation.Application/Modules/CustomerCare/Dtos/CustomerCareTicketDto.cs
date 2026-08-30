using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareTicketDto
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerPhone { get; set; }

    public string? CustomerAddress { get; set; }

    public CustomerCareTicketSource Source { get; set; }

    public CustomerCareTicketType Type { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int? SubCategoryId { get; set; }

    public string? SubCategoryName { get; set; }

    public string Description { get; set; } = string.Empty;

    public CustomerCareTicketPriority Priority { get; set; }

    public CustomerCareTicketStatus Status { get; set; }

    public int? AssignedToUserId { get; set; }

    public int? AssignedDepartmentId { get; set; }

    public string? AssignedDepartmentName { get; set; }

    public int? BranchId { get; set; }

    public int? SalesRepId { get; set; }

    public int? SupervisorId { get; set; }

    public DateTime? FirstResponseDueAt { get; set; }

    public DateTime? ResolutionDueAt { get; set; }

    public DateTime? NextFollowUpAt { get; set; }

    public bool IsFirstResponseBreached { get; set; }

    public bool IsResolutionBreached { get; set; }

    public bool IsCritical { get; set; }

    public string? ResolutionSummary { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public int ReopenCount { get; set; }
}