namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class AddCustomerCareTicketActionDto
{
    public string ActionType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsInternalNote { get; set; }

    public DateTime? FollowUpAt { get; set; }
}