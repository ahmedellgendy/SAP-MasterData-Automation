namespace MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

public class ProductDto
{
    public int Id { get; set; }

    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;

    public decimal? OutletSellingPrice { get; set; }
    public decimal? RetailSellingPrice { get; set; }

    public string? Currency { get; set; }

    public DateTime? MarketValidFrom { get; set; }
    public DateTime? MarketValidTo { get; set; }

    public DateTime? RetailValidFrom { get; set; }
    public DateTime? RetailValidTo { get; set; }

    public bool IsActive { get; set; }
}