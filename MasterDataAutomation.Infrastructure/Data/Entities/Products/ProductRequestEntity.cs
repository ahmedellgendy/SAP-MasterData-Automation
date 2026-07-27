using MasterDataAutomation.Application.Modules.ProductMaster.Enums;

namespace MasterDataAutomation.Infrastructure.Data.Entities.Products;

public class ProductRequestEntity
{
    public int Id { get; set; }

    // Basic Product Data - entered by Accounts User
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Accounts Data
    public decimal PurchasePrice { get; set; }
    public int PiecesCount { get; set; }
    public decimal PiecePrice { get; set; }

    public bool HasBonus { get; set; }
    public int? BonusQuantity { get; set; }

    // Executive Pricing
    public decimal? OutletSellingPrice { get; set; }
    public decimal? RetailSellingPrice { get; set; }

    public string Currency { get; set; } = "EGP";

    // Workflow
    public ProductRequestStatus Status { get; set; } = ProductRequestStatus.Draft;
    public string? RejectionReason { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? SubmittedAt { get; set; }

    public DateTime? AccountsManagerReviewedAt { get; set; }
    public DateTime? ExecutiveManagerReviewedAt { get; set; }

    public DateTime? CreatedAsProductAt { get; set; }
}