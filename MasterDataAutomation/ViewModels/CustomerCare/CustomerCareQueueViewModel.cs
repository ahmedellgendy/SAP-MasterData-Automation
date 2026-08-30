using MasterDataAutomation.Application.Modules.CustomerCare.Dtos;
using MasterDataAutomation.Domain.Modules.CustomerCare.Enums;

namespace MasterDataAutomation.Web.ViewModels.CustomerCare;

public class CustomerCareQueueViewModel
{
    public string? Search { get; set; }

    public CustomerCareTicketStatus? Status { get; set; }

    public CustomerCareTicketPriority? Priority { get; set; }

    public CustomerCareTicketSource? Source { get; set; }

    public int? CategoryId { get; set; }

    public bool OverdueOnly { get; set; }

    public bool FollowUpOnly { get; set; }

    public List<CustomerCareLookupDto> Categories { get; set; } = new();

    public List<CustomerCareTicketListItemDto> Tickets { get; set; } = new();
}