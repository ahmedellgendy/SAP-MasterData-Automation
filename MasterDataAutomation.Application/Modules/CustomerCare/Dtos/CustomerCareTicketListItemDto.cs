using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareTicketListItemDto
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerPhone { get; set; }

    public CustomerCareTicketSource Source { get; set; }

    public CustomerCareTicketType Type { get; set; }

    public string? CategoryName { get; set; }

    public string? SubCategoryName { get; set; }

    public CustomerCareTicketPriority Priority { get; set; }

    public CustomerCareTicketStatus Status { get; set; }

    public int? AssignedToUserId { get; set; }

    public int? AssignedDepartmentId { get; set; }

    public string? AssignedDepartmentName { get; set; }

    public DateTime? ResolutionDueAt { get; set; }

    public DateTime? NextFollowUpAt { get; set; }

    public bool IsCritical { get; set; }

    public bool IsOverdue { get; set; }
}