using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CreateCustomerCareTicketDto
{
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
    // Details
    // =========================================================

    public string Description { get; set; } = string.Empty;

    public CustomerCareTicketPriority Priority { get; set; }
        = CustomerCareTicketPriority.Normal;

    // =========================================================
    // Assignment Context
    // =========================================================

    public int? AssignedToUserId { get; set; }

    public int? AssignedDepartmentId { get; set; }

    // =========================================================
    // Commercial Context
    // =========================================================

    public int? BranchId { get; set; }

    public int? SalesRepId { get; set; }

    public int? SupervisorId { get; set; }

    // =========================================================
    // Follow-up
    // =========================================================

    public DateTime? NextFollowUpAt { get; set; }

    // =========================================================
    // Quality
    // =========================================================

    public CreateCustomerCareQualityDetailDto? QualityDetail { get; set; }
}