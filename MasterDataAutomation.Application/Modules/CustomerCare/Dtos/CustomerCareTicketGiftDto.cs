namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class CustomerCareTicketGiftDto
{
    public int Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string? RecipientName { get; set; }

    public string? RecipientPhone { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public string CreatedByUserName { get; set; } = string.Empty;
}