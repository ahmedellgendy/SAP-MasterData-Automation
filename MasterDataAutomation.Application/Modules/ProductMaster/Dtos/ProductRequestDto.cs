using MasterDataAutomation.Application.Modules.ProductMaster.Enums;

namespace MasterDataAutomation.Application.Modules.ProductMaster.Dtos;

public class ProductRequestDto
{
    public int Id { get; set; }

    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public decimal PurchasePrice { get; set; }
    public int PiecesCount { get; set; }
    public decimal PiecePrice { get; set; }

    public bool HasBonus { get; set; }
    public int? BonusQuantity { get; set; }

    public decimal? OutletSellingPrice { get; set; }
    public decimal? RetailSellingPrice { get; set; }

    public string Currency { get; set; } = "EGP";

    public ProductRequestStatus Status { get; set; }
    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}