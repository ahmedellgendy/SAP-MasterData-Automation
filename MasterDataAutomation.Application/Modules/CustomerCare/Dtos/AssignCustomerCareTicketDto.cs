namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class AssignCustomerCareTicketDto
{
    public int? AssignedToUserId { get; set; }

    public int? AssignedDepartmentId { get; set; }

    public string? Reason { get; set; }
}