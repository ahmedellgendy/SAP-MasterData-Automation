using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class ChangeCustomerCareTicketStatusDto
{
    public CustomerCareTicketStatus Status { get; set; }

    public string? Reason { get; set; }
}