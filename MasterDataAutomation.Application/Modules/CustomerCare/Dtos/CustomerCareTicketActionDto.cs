namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareTicketActionDto
{
    public int Id { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsInternalNote { get; set; }

    public DateTime? FollowUpAt { get; set; }

    public int CreatedByUserId { get; set; }

    public string CreatedByUserName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}