namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareDuplicateCustomerDto
{
    public string CustomerPhone { get; set; } = string.Empty;

    public int TotalTickets { get; set; }

    public int OpenTickets { get; set; }

    public int TicketsLast30Days { get; set; }

    public List<CustomerCareTicketListItemDto> RecentTickets { get; set; }
        = new();
}