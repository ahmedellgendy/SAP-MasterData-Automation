namespace MasterDataAutomation.Domain.Modules.CustomerCare.Entities;

public class CustomerCareTicketGift
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string? RecipientName { get; set; }

    public string? RecipientPhone { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CreatedByUserId { get; set; }

    public string CreatedByUserName { get; set; } = string.Empty;

    public CustomerCareTicket Ticket { get; set; } = null!;
}