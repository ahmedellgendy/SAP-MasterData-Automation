namespace MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

public class CreateProductRequestDto
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public decimal PurchasePrice { get; set; }
    public int PiecesCount { get; set; }

    public bool HasBonus { get; set; }
    public int? BonusQuantity { get; set; }

    public string Currency { get; set; } = "EGP";
}