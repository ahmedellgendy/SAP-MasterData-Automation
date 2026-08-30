using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareTicketFilterDto
{
    public string? Search { get; set; }

    public CustomerCareTicketStatus? Status { get; set; }

    public CustomerCareTicketPriority? Priority { get; set; }

    public CustomerCareTicketSource? Source { get; set; }

    public CustomerCareTicketType? Type { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public int? AssignedToUserId { get; set; }

    public int? AssignedDepartmentId { get; set; }

    public int? BranchId { get; set; }

    public bool? IsCritical { get; set; }

    public bool OverdueOnly { get; set; }

    public bool FollowUpOnly { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
    public int? CreatedByUserId { get; set; }
}