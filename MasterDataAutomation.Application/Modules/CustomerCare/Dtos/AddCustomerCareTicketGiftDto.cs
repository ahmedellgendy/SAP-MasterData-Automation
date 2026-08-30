namespace MasterDataAutomation.Application.Modules.CustomerCare.Dtos;

public class AddCustomerCareTicketGiftDto
{
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string? RecipientName { get; set; }

    public string? RecipientPhone { get; set; }

    public string? Notes { get; set; }
}