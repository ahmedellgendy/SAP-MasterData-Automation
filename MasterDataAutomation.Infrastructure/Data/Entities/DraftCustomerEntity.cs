namespace MasterDataAutomation.Infrastructure.Data.Entities;

public class DraftCustomerEntity
{
    public int Id { get; set; }

    public string Line { get; set; } = string.Empty;

    public string Market { get; set; } = string.Empty;

    public string Branch { get; set; } = string.Empty;

    public string SalesDistrict { get; set; } = string.Empty;

    public string CustomerType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Draft";
    public string? RejectionReason { get; set; }
}