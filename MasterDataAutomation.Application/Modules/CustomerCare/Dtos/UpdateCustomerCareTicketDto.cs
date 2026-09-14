using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class UpdateCustomerCareTicketDto
{
    public string CustomerName { get; set; } = string.Empty;

    public string? CustomerPhone { get; set; }

    public string? CustomerAddress { get; set; }

    public CustomerCareTicketSource Source { get; set; }

    public CustomerCareTicketType Type { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public string Description { get; set; } = string.Empty;

    public CustomerCareTicketPriority Priority { get; set; }

    public DateTime? NextFollowUpAt { get; set; }
}